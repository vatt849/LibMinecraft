using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model;
using System.Net.Sockets;
using System.Threading;
using LibMinecraft.Model.Entities;
using System.Net;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Client
{
    /// <summary>
    /// MultiplayerService is a service that will
    /// handle communication between a client and 
    /// a server.
    /// </summary>
    /// <remarks></remarks>
    public class MultiplayerClient
    {
        // References
        // http://www.wiki.vg/Protocol
        // http://www.wiki.vg/How_to_Write_a_Client

        #region Properties
        MinecraftServer _currentServer = null;
        /// <summary>
        /// The current server being connected to.
        /// </summary>
        /// <value>The current server.</value>
        /// <remarks></remarks>
        public MinecraftServer CurrentServer
        {
            get
            {
                return _currentServer;
            }
            protected set
            {
                _currentServer = value;
            }
        }

        /// <summary>
        /// The amount of time (in milliseconds) between ticks
        /// </summary>
        /// <value>The tick time.</value>
        /// <remarks></remarks>
        public int TickTime { get; set; }

        /// <summary>
        /// If true, logging is much more
        /// verbose.
        /// </summary>
        /// <value><c>true</c> if [debug mode]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool DebugMode { get; set; }

        /// <summary>
        /// True if the client is currently connected to
        /// a Minecraft server.
        /// </summary>
        /// <remarks></remarks>
        public bool Connected
        {
            get
            {
                if (netSocket == null)
                    return false;
                return netSocket.Connected;
            }
        }
        #endregion

        #region Variables

        /// <summary>
        /// Used to read and write messages to and from
        /// the server.
        /// </summary>
        protected NetworkStream netStream = null;
        /// <summary>
        /// Maintains a connection to the server.
        /// </summary>
        protected Socket netSocket;
        /// <summary>
        /// The worker thread, which handles network activity
        /// </summary>
        protected Timer worker = null;
        /// <summary>
        /// Used to keep thread safety when working
        /// against the network.
        /// </summary>
        protected object lockObject = new object();
        /// <summary>
        /// 
        /// </summary>
        static Inflater inflater = new Inflater();
        /// <summary>
        /// 
        /// </summary>
        private string ServerID;
        /// <summary>
        /// 
        /// </summary>
        private string SessionID;

        #region Constants

        /// <summary>
        /// 
        /// </summary>
        private const int ProtocolVersion = 23;
        /// <summary>
        /// 
        /// </summary>
        private const string SessionIDUrl = "http://login.minecraft.net/?user={0}&password={1}&version=12";
        /// <summary>
        /// 
        /// </summary>
        private const string ServerIDUrl = "http://session.minecraft.net/game/joinserver.jsp?user={0}&sessionId={1}&serverId={2}";

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for MultiplayerClient
        /// </summary>
        /// <remarks></remarks>
        public MultiplayerClient()
        {
            TickTime = 50;
        }

        #endregion

        #region Server Information

        /// <summary>
        /// 
        /// </summary>
        private World _world;

        /// <summary>
        /// All known information about the current server's world
        /// </summary>
        /// <value>The world.</value>
        /// <remarks></remarks>
        public World World
        {
            get
            {
                return _world;
            }
            protected set
            {
                _world = value;
            }
        }

        /// <summary>
        /// The current player on the server
        /// </summary>
        /// <value>The player.</value>
        /// <remarks></remarks>
        public PlayerEntity Player { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private List<Entity> _entities;

        /// <summary>
        /// All entities the client is currently
        /// aware of.
        /// </summary>
        /// <value>The entities.</value>
        /// <remarks></remarks>
        public List<Entity> Entities
        {
            get
            {
                return _entities;
            }
            protected set
            {
                _entities = value;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when the client recieves a message from the server
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<ServerMessageEventArgs> OnServerMessage;

        /// <summary>
        /// Called when the client recieves chat from the server
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<ChatEventArgs> OnChat;

        /// <summary>
        /// Called when the client dies
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler OnPlayerDeath;

        /// <summary>
        /// Called every tick
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler OnTick;

        /// <summary>
        /// Called when the client disconnects from the server
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler OnDisconnect;

        /// <summary>
        /// Called when the client's health changes
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<HealthUpdateEventArgs> OnHealthChange;

        /// <summary>
        /// Called when the client's hunger changes
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<HungerUpdateEventArgs> OnHungerChange;

        #endregion

        #region Worker

        // Switch Vars
        /// <summary>
        /// 
        /// </summary>
        private short itemid, length;
        /// <summary>
        /// 
        /// </summary>
        private int size, count;
        /// <summary>
        /// 
        /// </summary>
        private String s;

        /// <summary>
        /// Does the work.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <remarks></remarks>
        private void DoWork(object state)
        {
            lock (lockObject)
            {
                try
                {
                    if (netStream == null)
                        return;
                    if (OnTick != null)
                        OnTick(this, null);
                    // Retrieve new data
                    // TODO: Switch to LibMinecraft.Model.Packet
                    while (netStream.DataAvailable)
                    {
                        PacketID messageType = (PacketID)netStream.ReadByte();
                        if (OnServerMessage != null)
                            OnServerMessage(this, new ServerMessageEventArgs());
                        switch (messageType)
                        {
                            case PacketID.KeepAlive:
                                netStream.WriteByte((byte)PacketID.KeepAlive);
                                WriteInt(netStream, ReadInt(netStream));
                                Console.WriteLine("Keep alive");
                                break;
                            case PacketID.ChatUpdate:
                                s = ReadString(netStream);
                                if (OnChat != null)
                                    OnChat(this, new ChatEventArgs(s));
                                break;
                            case PacketID.TimeUpdate:
                                World.Level.Time = ReadLong(netStream);
                                break;
                            case PacketID.EntityEquipment: ReadBytes(netStream, 10); break;
                            case PacketID.SpawnPosition:
                                World.Level.Spawn = ReadVector3(netStream);
                                break;
                            case PacketID.UpdateHealth:
                                HealthUpdateEventArgs e = new HealthUpdateEventArgs(Player.Health, ReadShort(netStream));
                                HungerUpdateEventArgs he = new HungerUpdateEventArgs(Player.Food, ReadShort(netStream));
                                Player.Health = e.Health;
                                Player.Food = he.Hunger;
                                Player.FoodSaturation = ReadFloat(netStream); // ReadFloat might not work right
                                if (e.Health != e.OldHealth)
                                    if (OnHealthChange != null)
                                        OnHealthChange(this, e);
                                if (he.Hunger != he.OldHunger)
                                    if (OnHungerChange != null)
                                        OnHungerChange(this, he);
                                if (Player.Health <= 0)
                                {
                                    if (OnPlayerDeath != null)
                                        OnPlayerDeath(this, new EventArgs());
                                }
                                break;
                            case PacketID.Respawn:
                                ReadBytes(netStream, 13);
                                ReadString(netStream);
                                break;
                            case PacketID.PlayerPosition: ReadBytes(netStream, 33); break;
                            case PacketID.PlayerLook: ReadBytes(netStream, 9); break;
                            case PacketID.PlayerPositionAndLook:
                                Player.Location.X = ReadDouble(netStream);
                                Player.Stance = ReadDouble(netStream);
                                Player.Location.Y = ReadDouble(netStream);
                                Player.Location.Z = ReadDouble(netStream);
                                Player.Rotation.X = ReadFloat(netStream);
                                Player.Rotation.Y = ReadFloat(netStream);
                                Player.OnGround = ReadBoolean(netStream);

                                SendPlayerPositionAndLook(); // the server is unhappy with us, we are reporting the wrong place
                                break;
                            case PacketID.PlayerDigging: ReadBytes(netStream, 11); break;
                            case PacketID.PlayerBlockPlacement:
                                ReadInt(netStream);
                                ReadBytes(netStream, 1);
                                ReadInt(netStream);
                                ReadBytes(netStream, 1);
                                itemid = ReadShort(netStream);
                                if (itemid >= 0)
                                {
                                    GZipStream gzstream = new GZipStream(netStream, CompressionMode.Decompress);
                                    byte amount = ReadBytes(netStream, 1)[0];
                                    short damage = ReadShort(netStream);
                                }
                                break;
                            case PacketID.HeldItemChange: ReadBytes(netStream, 2); break;
                            case PacketID.UseBed: ReadBytes(netStream, 14); break;
                            case PacketID.Animation: ReadBytes(netStream, 5); break;
                            case PacketID.EntityAction: ReadBytes(netStream, 5); break;
                            case PacketID.SpawnNamedEntity:
                                ReadInt(netStream);
                                ReadString(netStream);
                                ReadInt(netStream);
                                ReadInt(netStream);
                                ReadInt(netStream);
                                ReadBytes(netStream, 2);
                                ReadShort(netStream);
                                break;
                            case PacketID.SpawnDroppedItem: ReadBytes(netStream, 24); break;
                            case PacketID.CollectItem: ReadBytes(netStream, 8); break;
                            case PacketID.SpawnObjectOrVehicle:
                                int eid = ReadInt(netStream);
                                byte type = (byte)netStream.ReadByte();
                                Vector3 pos = ReadVector3(netStream);
                                int fireID = ReadInt(netStream);
                                if (fireID > 0)
                                    ReadVector3(netStream);
                                break;
                            case PacketID.SpawnMob:
                                ReadBytes(netStream, 19);
                                while ((byte)netStream.ReadByte() != 0x7f) { } // Metadata
                                break;
                            case PacketID.SpawnPainting:
                                ReadInt(netStream);
                                ReadString(netStream);
                                ReadInt(netStream);
                                ReadInt(netStream);
                                ReadInt(netStream);
                                ReadInt(netStream);
                                break;
                            case PacketID.SpawnExperienceOrb: ReadBytes(netStream, 18); break;
                            case PacketID.EntityVelocity: ReadBytes(netStream, 10); break;
                            case PacketID.DestroyEntity: ReadBytes(netStream, 4); break;
                            case PacketID.Entity: ReadBytes(netStream, 4); break;
                            case PacketID.EntityRelativeMove: ReadBytes(netStream, 7); Console.WriteLine("Entity relative move"); break;
                            case PacketID.EntityLook: ReadBytes(netStream, 6); break;
                            case PacketID.EntityLookAndRelativeMove: ReadBytes(netStream, 9); Console.WriteLine("Entity look/relative move"); break;
                            case PacketID.EntityTeleport:
                                ReadBytes(netStream, 14);
                                Console.WriteLine(netStream.ReadByte());
                                Console.WriteLine("Entity teleport");
                                break;
                            case PacketID.EntityStatus: ReadBytes(netStream, 5); break;
                            case PacketID.AttachEntity: ReadBytes(netStream, 8); break;
                            case PacketID.EntityMetadata:
                                ReadInt(netStream);
                                while ((byte)netStream.ReadByte() != 0x7f) { } // Metadata
                                break;
                            case PacketID.EntityEffect: ReadBytes(netStream, 8); break;
                            case PacketID.RemoveEntityEffect: ReadBytes(netStream, 5); break;
                            case PacketID.SetExperience: ReadBytes(netStream, 8); break;
                            case PacketID.MapColumnAllocation:
                              
                                    var chunkPos = new Vector3()
                                                       {
                                                           X = ReadInt(netStream),
                                                           Z = ReadInt(netStream),
                                                       };

                                    var mode = ReadBoolean(netStream);
                                    if (mode == false)
                                    {
                                        //if (World.IsChunkLoaded(chunkPos))
                                        //{
                                        //    World.Chunks.Remove(World.GetChunk(chunkPos));
                                        //    GC.Collect();
                                        //}
                                    }
                                
                                break;
                            case PacketID.MapChunks:
                              
                                     chunkPos = new Vector3();
                                    chunkPos.X = ReadInt(netStream);
                                    chunkPos.Y = ReadShort(netStream);
                                    chunkPos.Z = ReadInt(netStream);
                                    byte chunkSizeX = (byte) netStream.ReadByte();
                                    byte chunkSizeY = (byte) netStream.ReadByte();
                                    byte chunkSizeZ = (byte) netStream.ReadByte();
                                    size = ReadInt(netStream);
                                    byte[] deflatedChunk = ReadBytes(netStream, size);

                                    Chunk chunk = new Chunk(chunkPos);

                                    InflaterInputStream inflateStream =
                                        new InflaterInputStream(new MemoryStream(deflatedChunk));
                                    byte[] inflatedChunk = new byte[(int) (chunkSizeX*chunkSizeY*chunkSizeZ*2.5)];
                                    inflateStream.Read(inflatedChunk, 0, inflatedChunk.Length);
                                    inflateStream.Close();

                                    try
                                    {
                                        Array.Copy(inflatedChunk, 0, chunk.Blocks, 0, chunk.Blocks.Length);

                                        byte[] meta = new byte[chunk.Blocks.Length/2];
                                        Array.Copy(inflatedChunk, chunk.Blocks.Length, meta, 0, meta.Length);
                                        chunk.SetMetadata(meta);
                                        //World.SetChunk(chunk);
                                        break;

                                        //Array.Copy(inflatedChunk, 0, chunk.Blocks, 0, chunk.Blocks.Length);
                                        //byte[] blockLight = new byte[chunk.Blocks.Length/2];
                                        //Array.Copy(inflatedChunk, chunk.Blocks.Length + meta.Length, blockLight, 0,
                                        //           blockLight.Length);
                                        //chunk.SetBlockLight(blockLight);

                                        //Array.Copy(inflatedChunk, 0, chunk.Blocks, 0, chunk.Blocks.Length);
                                        //byte[] skyLight = new byte[chunk.Blocks.Length/2];
                                        //Array.Copy(inflatedChunk, chunk.Blocks.Length + meta.Length + blockLight.Length,
                                        //           skyLight, 0, skyLight.Length);
                                        //chunk.SetSkyLight(skyLight);
                                    }
                                    catch
                                    {
                                    }

                                    //World.SetChunk(chunk);
                                
                                break;
                            case PacketID.MultiBlockChange:
                                ReadInt(netStream);
                                ReadInt(netStream);
                                length = ReadShort(netStream); // Number of elements per array
                                ReadBytes(netStream, length * 2); // Short array (Coordinates)
                                ReadBytes(netStream, length); // Byte array (Types)
                                ReadBytes(netStream, length); // Byte array (Metadata)
                                break;
                            case PacketID.BlockChange:
                                pos = new Vector3();
                                pos.X = ReadInt(netStream);
                                pos.Y = (byte)netStream.ReadByte();
                                pos.Z = ReadInt(netStream);
                                Block newBlock = (byte)netStream.ReadByte();
                                newBlock.Metadata = (byte)netStream.ReadByte();
                                World.SetBlock(pos, newBlock);
                                break;
                            case PacketID.BlockAction: ReadBytes(netStream, 12); break;
                            case PacketID.Explosion:
                                ReadDouble(netStream);
                                ReadDouble(netStream);
                                ReadDouble(netStream);
                                ReadFloat(netStream);
                                count = ReadInt(netStream);
                                ReadBytes(netStream, count * 3);
                                break;
                            case PacketID.SoundOrParticleEffect: ReadBytes(netStream, 17); break;
                            case PacketID.ChangeGameState: ReadBytes(netStream, 2); break;
                            case PacketID.Thunderbolt: ReadBytes(netStream, 17); break;
                            case PacketID.OpenWindow:
                                netStream.ReadByte();
                                netStream.ReadByte();
                                ReadString(netStream);
                                netStream.ReadByte();
                                break;
                            case PacketID.SetSlot:
                                netStream.ReadByte();
                                ReadShort(netStream);
                                short item = ReadShort(netStream);
                                if (item != -1)
                                {
                                    netStream.ReadByte();
                                    ReadShort(netStream);
                                }
                                break;
                            case PacketID.SetWindowItems:
                                netStream.ReadByte(); // Window ID
                                short c = ReadShort(netStream); // Count
                                for (int i = 0; i < c; i++) // Payload
                                {
                                    itemid = ReadShort(netStream);
                                    if (itemid != -1)
                                    {
                                        netStream.ReadByte();
                                        ReadShort(netStream);
                                    }
                                }
                                break;
                            case PacketID.UpdateWindowProperty: ReadBytes(netStream, 5); break;
                            case PacketID.ConfirmTransaction: ReadBytes(netStream, 4); break;
                            case PacketID.CreativeInventoryAction: ReadBytes(netStream, 8); break;
                            case PacketID.UpdateSign:
                                ReadInt(netStream);
                                ReadShort(netStream);
                                ReadInt(netStream);
                                ReadString(netStream);
                                ReadString(netStream);
                                ReadString(netStream);
                                ReadString(netStream);
                                break;
                            case PacketID.ItemData:
                                ReadShort(netStream);
                                ReadShort(netStream);
                                byte l = (byte)netStream.ReadByte();
                                ReadBytes(netStream, l);
                                break;
                            case PacketID.IncrementStatistic: ReadBytes(netStream, 5); break;
                            case PacketID.PluginMessage:
                                string unknownString = ReadString(netStream);
                                short unknownLength = ReadShort(netStream);
                                Read(netStream, unknownLength);
                                Console.WriteLine("Handled 0xFA packet from server: " + unknownString + " [" + unknownLength + "]");
                                break;
                            case PacketID.PlayerListItem:
                                string player = ReadString(netStream);
                                bool online = ReadBoolean(netStream);
                                short ping = ReadShort(netStream);
                                Console.WriteLine("PlayerListItem: \n" + player + "\nOnline: " + online.ToString() + "\nPing: " + ping.ToString());
                                break;
                            case PacketID.Disconnect:
                                String reason = ReadString(netStream);
                                Log("Disconnecting: " + reason);
                                Disconnect();
                                return;
                        }
                    }
                    // Send updated information
                    SendPlayerPositionAndLook();
                }
                catch
                {
                    try
                    {
                        Disconnect();
                    }
                    catch
                    {
                        ForceDisconnect();
                    }
                }
            }
        }

        #endregion

        #region Connection/Log In

        /// <summary>
        /// Connects to the specified server (does not log in)
        /// </summary>
        /// <param name="server">The server.</param>
        /// <remarks></remarks>
        public void Connect(MinecraftServer server)
        {
            if (CurrentServer != null)
                throw new Exception("You cannot connect to another server before disconnecting.");
            try
            {
                CurrentServer = server;
                netSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                netSocket.Connect(server.IP, server.Port);
                netStream = new NetworkStream(netSocket);
            }
            catch (Exception e)
            {
                try
                {
                    Disconnect();
                }
                catch { }
                finally
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        /// <remarks></remarks>
        public void Disconnect()
        {
            if (CurrentServer == null)
                throw new Exception("You must connect before disconnecting.");
            if (worker != null)
                worker.Dispose();
            worker = null;
            if (netSocket.Connected)
                SendDisconnect();
            CurrentServer = null;
            if (netSocket.Connected)
                netStream.Close();
            netSocket.Close();
            netSocket = null;
            netStream = null;
            if (OnDisconnect != null)
                OnDisconnect(this, new EventArgs());
        }

        /// <summary>
        /// Forcibly disconnects from a server.
        /// </summary>
        /// <remarks></remarks>
        public void ForceDisconnect()
        {
            try
            {
                if (worker != null)
                    worker.Dispose();
                worker = null;
                CurrentServer = null;
                if (netSocket.Connected)
                    netStream.Close();
                netStream = null;
            }
            catch
            {
                worker = null;
                CurrentServer = null;
                netStream = null;
            }
            if (OnDisconnect != null)
                OnDisconnect(this, new EventArgs());
        }

        /// <summary>
        /// Logs the specified user into the server
        /// </summary>
        /// <param name="user">The user.</param>
        /// <remarks></remarks>
        public void LogIn(User user)
        {
            if (CurrentServer == null)
                throw new Exception("You must connect to a server before logging in.");
            if (user.UserName.Length > 16)
                throw new Exception("User name is too long.  The maximum length is 16 characters.");

            SendHandshake(user);
            SendLogin(user, ProtocolVersion);

            // Start server updater
            worker = new Timer(new TimerCallback(DoWork), null, 0, TickTime);
        }

        #endregion

        #region Packets

        /// <summary>
        /// Sends a handshake packet
        /// </summary>
        /// <param name="user">The user.</param>
        /// <remarks></remarks>
        protected void SendHandshake(User user)
        {
            lock (lockObject)
            {
                WebClient c = new WebClient();
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    SessionID = c.DownloadString(string.Format(SessionIDUrl, user.UserName, user.Password));
                    if (SessionID == "Bad Login")
                        throw new Exception("Invalid credentials.");
                    string[] sessionValues = SessionID.Split(':');
                    user.UserName = sessionValues[2];
                    SessionID = sessionValues[3];
                }

                // Do handshake
                netStream.WriteByte((byte)PacketID.Handshake);
                WriteString(netStream, user.UserName);
                netStream.Flush();

                PacketID response = (PacketID)netStream.ReadByte();

                if (response != PacketID.Handshake)
                    throw new Exception("Unrecognized response from server.");

                ServerID = ReadString(netStream);

                if (ServerID == "-")
                {
                    // No further action required as of 1.1
                }
                else if (ServerID == "+")
                {
                    // Password authentication required (presently unsupported by official
                    // software, so it's unsupported by us, too)
                }
                else
                {
                    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(string.Format(ServerIDUrl, user.UserName, SessionID, ServerID));
                    try
                    {
                        wr.GetResponse().Close();
                    }
                    catch
                    {
                        throw new Exception("Failed to authenticate with the server.");
                    }
                }
            }
        }

        /// <summary>
        /// Sends a login packet
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="protocolVersion">The protocol version.</param>
        /// <remarks></remarks>
        protected void SendLogin(User user, int protocolVersion)
        {
            lock (lockObject)
            {
                // Send login packet
                netStream.WriteByte((byte)PacketID.LoginRequest);
                WriteInt(netStream, protocolVersion);
                WriteString(netStream, user.UserName);
                WriteLong(netStream, 0);
                WriteString(netStream, "");
                WriteInt(netStream, 0);
                netStream.WriteByte(0);
                netStream.WriteByte(0);
                netStream.WriteByte(0);
                netStream.WriteByte(0);
                netStream.Flush();

                // Receive response
                PacketID response = (PacketID)netStream.ReadByte();

                if (response == PacketID.Disconnect)
                {
                    ForceDisconnect();
                    throw new Exception(ReadString(netStream));
                }

                if (response != PacketID.LoginRequest)
                {
                    string dump = ReadString(netStream);
                    throw new Exception("Unrecognized response from server.");
                }

                Player = new PlayerEntity();
                //World = new World();
                //Player.ID = ReadInt(netStream);
                //ReadString(netStream);
                //World.Seed = ReadLong(netStream);
                //World.WorldGenerator = null;
                //World.Mode = (GameMode)ReadInt(netStream);
                //Player.Dimension = (Dimension)((sbyte)netStream.ReadByte());
                //CurrentServer.Difficulty = (Difficulty)netStream.ReadByte();
                //(byte)netStream.ReadByte();
                CurrentServer.MaxPlayers = netStream.ReadByte();
                netStream.Flush();
                Player.Name = user.UserName;
            }
        }

        /// <summary>
        /// Sends a position and look packet
        /// </summary>
        /// <remarks></remarks>
        public void SendPlayerPositionAndLook()
        {
            Player.Stance = 1 + Player.Location.Y;
            netStream.WriteByte((byte)PacketID.PlayerPositionAndLook);
            WriteDouble(netStream, Player.Location.X);
            WriteDouble(netStream, Player.Location.Y);
            WriteDouble(netStream, Player.Stance);
            WriteDouble(netStream, Player.Location.Z);
            WriteFloat(netStream, (float)Player.Rotation.X);
            WriteFloat(netStream, (float)Player.Rotation.Y);
            WriteBoolean(netStream, Player.OnGround);
        }

        /// <summary>
        /// Sends a disconnect packet
        /// </summary>
        /// <remarks></remarks>
        protected void SendDisconnect()
        {
            netStream.WriteByte((byte)PacketID.Disconnect);
            netStream.Flush();
            if (OnDisconnect != null)
                OnDisconnect(this, new EventArgs());
        }

        /// <summary>
        /// Sends a chat message to the server.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <remarks></remarks>
        public void SendChat(string Message)
        {
            if (CurrentServer == null)
                throw new Exception("Must be connected to a server to chat.");
            lock (lockObject)
            {
                if (Message.Length > 100)
                    throw new Exception("Chat message exceeded maximum length");
                netStream.WriteByte((byte)PacketID.ChatUpdate);
                WriteString(netStream, Message);
                netStream.Flush();
            }
        }

        /// <summary>
        /// Respawns after the player's death
        /// </summary>
        /// <remarks></remarks>
        public void Respawn()
        {
            if (Player.Health > 0)
                throw new Exception("Player is not dead!");
            lock (lockObject)
            {
                netStream.WriteByte((byte)PacketID.Respawn);
                netStream.WriteByte((byte)Player.Dimension);
                netStream.WriteByte((byte)CurrentServer.Difficulty);
                netStream.WriteByte((byte)World.Level.GameMode);
                WriteShort(netStream, World.Height);
                WriteLong(netStream, World.Level.Seed);
                WriteString(netStream, World.Level.WorldGenerator.Name);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Logs the specified text.
        /// </summary>
        /// <param name="Text">The text.</param>
        /// <remarks></remarks>
        private void Log(string Text)
        {
            if (DebugMode)
            {
                Console.WriteLine(Text);
            }
        }

        /// <summary>
        /// Logs the specified text.
        /// </summary>
        /// <param name="Text">The text.</param>
        /// <param name="obj">The obj.</param>
        /// <remarks></remarks>
        private void Log(string Text, params object[] obj)
        {
            if (DebugMode)
            {
                Console.WriteLine(Text, obj);
            }
        }

        /// <summary>
        /// Reads the NBT.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static Dictionary<string, byte[]> ReadNBT()
        {
            Dictionary<string, byte[]> value = new Dictionary<string, byte[]>();

            return value;
        }

        /// <summary>
        /// Reads the int.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static int ReadInt(Stream s)
        {
            return IPAddress.HostToNetworkOrder((int)Read(s, 4));
        }

        /// <summary>
        /// Reads the short.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static short ReadShort(Stream s)
        {
            return IPAddress.HostToNetworkOrder((short)Read(s, 2));
        }

        /// <summary>
        /// Reads the long.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static long ReadLong(Stream s)
        {
            return IPAddress.HostToNetworkOrder((long)Read(s, 8));
        }

        /// <summary>
        /// Reads the double.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static double ReadDouble(Stream s)
        {
            byte[] doubleArray = new byte[sizeof(double)];
            s.Read(doubleArray, 0, sizeof(double));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(doubleArray);
            return BitConverter.ToDouble(doubleArray, 0);
        }

        /// <summary>
        /// Reads the float.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static float ReadFloat(Stream s)
        {
            byte[] floatArray = new byte[sizeof(float)];
            s.Read(floatArray, 0, sizeof(float));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(floatArray);
            return BitConverter.ToSingle(floatArray, 0);
        }

        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static Boolean ReadBoolean(Stream s)
        {
            return new BinaryReader(s).ReadBoolean();
        }

        /// <summary>
        /// Reads the bytes.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static byte[] ReadBytes(Stream s, int count)
        {
            return new BinaryReader(s).ReadBytes(count);
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static String ReadString(Stream s)
        {
            short len;
            byte[] a = new byte[2];
            a[0] = (byte)s.ReadByte();
            a[1] = (byte)s.ReadByte();
            len = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(a, 0));
            byte[] b = new byte[len * 2];
            for (int i = 0; i < len * 2; i++)
            {
                b[i] = (byte)s.ReadByte();
            }
            return Encoding.BigEndianUnicode.GetString(b);
        }

        /// <summary>
        /// Reads a Vector3 from a stream and ajdusts the byte order
        /// </summary>
        /// <param name="s">The stream to read from</param>
        /// <returns>Adjusted Vector3</returns>
        /// <remarks></remarks>
        protected Vector3 ReadVector3(Stream s)
        {
            return new Vector3(ReadInt(s), ReadInt(s), ReadInt(s));
        }

        /// <summary>
        /// Writes a Vector3 in integer form to
        /// a stream.
        /// </summary>
        /// <param name="s">The stream to write to</param>
        /// <param name="vector">The Vector3 to write</param>
        /// <remarks></remarks>
        protected void WriteVector3(Stream s, Vector3 vector)
        {
            WriteInt(s, (int)vector.X);
            WriteInt(s, (int)vector.Y);
            WriteInt(s, (int)vector.Z);
        }

        /// <summary>
        /// Writes a string in Big-Endian Unicode format
        /// to the specified stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="msg">The MSG.</param>
        /// <remarks></remarks>
        public static void WriteString(Stream s, string msg)
        {

            short len = IPAddress.HostToNetworkOrder((short)msg.Length);
            byte[] a = BitConverter.GetBytes(len);
            byte[] b = Encoding.BigEndianUnicode.GetBytes(msg);
            byte[] c = a.Concat(b).ToArray();
            string dump = DumpArray(c);
            s.Write(c, 0, c.Length);
        }

        /// <summary>
        /// Writes an integer to the specified stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="i">The i.</param>
        /// <remarks></remarks>
        public static void WriteInt(Stream s, int i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        /// <summary>
        /// Writes a long to the specified stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="i">The i.</param>
        /// <remarks></remarks>
        public static void WriteLong(Stream s, long i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        /// <summary>
        /// Writes a short to the specified stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="i">The i.</param>
        /// <remarks></remarks>
        public static void WriteShort(Stream s, short i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        /// <summary>
        /// Writes a double to the specified stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="d">The d.</param>
        /// <remarks></remarks>
        public static void WriteDouble(Stream s, double d)
        {
            byte[] doubleArray = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(doubleArray);
            s.Write(doubleArray, 0, sizeof(double));
        }

        /// <summary>
        /// Writes a float to the specified stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="f">The f.</param>
        /// <remarks></remarks>
        public static void WriteFloat(Stream s, float f)
        {
            byte[] floatArray = BitConverter.GetBytes(f);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(floatArray);
            s.Write(floatArray, 0, sizeof(float));
        }

        /// <summary>
        /// Writes a boolean to the specified stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <remarks></remarks>
        public static void WriteBoolean(Stream s, Boolean b)
        {
            new BinaryWriter(s).Write(b);
        }

        /// <summary>
        /// Writes an array of bytes to the stream.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="b">The b.</param>
        /// <remarks></remarks>
        public static void WriteBytes(Stream s, byte[] b)
        {
            new BinaryWriter(s).Write(b); // TODO: better
        }

        /// <summary>
        /// Reads the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="num">The num.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static Object Read(Stream s, int num)
        {
            byte[] b = new byte[num];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)s.ReadByte();
            }
            switch (num)
            {
                case 4:
                    return BitConverter.ToInt32(b, 0);
                case 8:
                    return BitConverter.ToInt64(b, 0);
                case 2:
                    return BitConverter.ToInt16(b, 0);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Dumps the array.
        /// </summary>
        /// <param name="resp">The resp.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static string DumpArray(byte[] resp)
        {
            try
            {
                if (resp.Length == 0)
                    return "";
                string res = "";
                foreach (byte b in resp)
                    res += b.ToString("x2") + ":";
                return res.Remove(res.Length - 1);
            }
            catch { return ""; }
        }

        #endregion
    }

    /// <summary>
    /// Describes a message recieved from a server
    /// </summary>
    /// <remarks></remarks>
    public class ServerMessageEventArgs : EventArgs
    {

    }

    /// <summary>
    /// Describes a change in a player's health
    /// </summary>
    /// <remarks></remarks>
    public class HealthUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The player's health before the change
        /// </summary>
        /// <value>The old health.</value>
        /// <remarks></remarks>
        public short OldHealth { get; set; }
        /// <summary>
        /// The player's health after the change
        /// </summary>
        /// <value>The health.</value>
        /// <remarks></remarks>
        public short Health { get; set; }

        /// <summary>
        /// Default constructor for HealthUpdateEventArgs
        /// </summary>
        /// <param name="OldHealth">The old health.</param>
        /// <param name="Health">The health.</param>
        /// <remarks></remarks>
        public HealthUpdateEventArgs(short OldHealth, short Health)
        {
            this.OldHealth = OldHealth;
            this.Health = Health;
        }
    }

    /// <summary>
    /// Describes a change in a player's hunger
    /// </summary>
    /// <remarks></remarks>
    public class HungerUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// The player's hunger before the change
        /// </summary>
        /// <value>The old hunger.</value>
        /// <remarks></remarks>
        public short OldHunger { get; set; }
        /// <summary>
        /// The player's hunger after the change
        /// </summary>
        /// <value>The hunger.</value>
        /// <remarks></remarks>
        public short Hunger { get; set; }

        /// <summary>
        /// Default constructor for HealthUpdateEventArgs
        /// </summary>
        /// <param name="OldHunger">The old hunger.</param>
        /// <param name="Hunger">The hunger.</param>
        /// <remarks></remarks>
        public HungerUpdateEventArgs(short OldHunger, short Hunger)
        {
            this.OldHunger = OldHunger;
            this.Hunger = Hunger;
        }
    }
}
