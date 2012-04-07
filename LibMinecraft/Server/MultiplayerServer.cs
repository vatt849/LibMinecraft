using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using LibMinecraft.Client;
using LibMinecraft.Model.Entities;
using System.Security.Cryptography;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Model.Items;
using LibMinecraft.Model.Packets;
using System.Diagnostics;
using System.ComponentModel;

namespace LibMinecraft.Server
{
    /// <summary>
    /// Manages the network service provided
    /// by a Minecraft server.
    /// </summary>
    /// <remarks></remarks>
    public class MultiplayerServer
    {
        #region Properties

        /// <summary>
        /// The server being run.
        /// </summary>
        MinecraftServer _currentServer = null;
        /// <summary>
        /// Gets or sets the current server.
        /// </summary>
        /// <value>The current server.</value>
        /// <remarks></remarks>
        public MinecraftServer CurrentServer
        {
            get
            {
                return _currentServer;
            }
            set
            {
                _currentServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the tick interval.
        /// </summary>
        /// <value>The tick time.</value>
        /// <remarks></remarks>
        public int TickTime { get; set; }

        /// <summary>
        /// Gets or sets the list of connected clients.
        /// </summary>
        /// <value>The connected clients.</value>
        /// <remarks></remarks>
        public List<RemoteClient> ConnectedClients { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a packet log is enabled.
        /// This is extremely low-level packet logging, and is intended for
        /// debugging internally within LibMinecraft.
        /// </summary>
        /// <value><c>true</c> if packets should be logged otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool LogEnabled { get; set; }

        /// <summary>
        /// The list of all the Levels on this server.
        /// </summary>
        public List<Level> Levels;

        /// <summary>
        /// A global random generator.
        /// </summary>
        /// <value>The random generator.</value>
        /// <remarks></remarks>
        internal static Random Random { get; set; }

        #endregion

        #region Variables

        /// <summary>
        /// The TCP listener used to run the server.
        /// </summary>
        protected TcpListener netListener;
        /// <summary>
        /// The timer for network worker.
        /// </summary>
        protected Timer worker = null;
        /// <summary>
        /// The timer for in-game ticks.
        /// </summary>
        protected Timer tickTimer = null;
        /// <summary>
        /// This object may be used to avoid conflict when modifying player send queues.
        /// </summary>
        public object SendQueueLock = new object();
        /// <summary>
        /// The next Entity ID to be assigned.
        /// </summary>
        private static int _NextEntityID = 1;
        /// <summary>
        /// Number of ticks between world saves.
        /// Use -1 to disable.
        /// </summary>
        public int SaveFrequency = 100;
        /// <summary>
        /// Gets the next entity ID to be assigned.
        /// </summary>
        /// <remarks></remarks>
        public static int NextEntityID
        {
            get
            {
                return _NextEntityID;
            }
            internal set
            {
                _NextEntityID = value;
            }
        }
        /// <summary>
        /// If true, all packets will be output to the console
        /// and to a file.
        /// </summary>
        private const bool LogPackets = true;
        /// <summary>
        /// The time that the server started.
        /// </summary>
        private DateTime serverStartTime;
        /// <summary>
        /// The file to log packets to when LogPackets is true.
        /// </summary>
        private StreamWriter logFile;
        /// <summary>
        /// The singleton.
        /// </summary>
        internal static MultiplayerServer This; // TODO: Remove

        #region Constants

        /// <summary>
        /// The protocol version supported by this server.
        /// </summary>
        public const int ProtocolVersion = 29;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a player joins the server.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<PlayerEventArgs> OnPlayerJoin;
        /// <summary>
        /// Occurs when a player leaves the server.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<PlayerEventArgs> OnPlayerLeave;
        /// <summary>
        /// Occurs when any chat occurs.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<ChatEventArgs> OnChat;
        /// <summary>
        /// Called before a packet is handled.  If PacketEventArgs.Handled
        /// is set to true, then PostPacket will not be called and LibMinecraft
        /// will not handle the packet.  For incoming packets, the remaining
        /// packet has not yet been read from the network stream.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<PacketEventArgs> PrePacket;
        /// <summary>
        /// Called after a packet is handled.  PacketEventArgs.Handled is
        /// ignored.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<PacketEventArgs> PostPacket;
        /// <summary>
        /// Occurs on every tick.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler OnTick;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplayerServer"/> class.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <remarks></remarks>
        public MultiplayerServer(MinecraftServer Server)
        {
            CurrentServer = Server;
            TickTime = 50;
            ConnectedClients = new List<RemoteClient>();
            Levels = new List<Level>();
            this.keepAlive = new KeepAlivePacket(0);
            This = this;
            Random = new Random();
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Starts the Minecraft server
        /// </summary>
        /// <remarks></remarks>
        public void Start()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), CurrentServer.Port); // Grab the first available IPv4 endpoint
            CurrentServer.IP = (from a in ipHostInfo.AddressList
                                where a.AddressFamily == AddressFamily.InterNetwork
                                select a).First().ToString();

            netListener = new TcpListener(localEP);
            netListener.Start();

#if DEBUG
            if (LogEnabled)
            {
                if (!File.Exists("server.log"))
                    File.Create("server.log").Close();
                logFile = new StreamWriter("server.log", true);
            }
#endif

            serverStartTime = DateTime.Now;

            worker = new Timer(new TimerCallback(DoNet), null, TickTime, Timeout.Infinite);
            tickTimer = new Timer(new TimerCallback(DoTick), null, TickTime, Timeout.Infinite);
        }

        /// <summary>
        /// Starts the Minecraft server using the given
        /// local end point.
        /// </summary>
        /// <remarks></remarks>
        public void Start(IPEndPoint LocalEndPoint)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            CurrentServer.IP = (from a in ipHostInfo.AddressList
                                where a.AddressFamily == AddressFamily.InterNetwork
                                select a).First().ToString();

            netListener = new TcpListener(LocalEndPoint);
            netListener.Start();

#if DEBUG
            if (LogEnabled)
            {
                if (!File.Exists("server.log"))
                    File.Create("server.log").Close();
                logFile = new StreamWriter("server.log", true);
            }
#endif

            serverStartTime = DateTime.Now;

            worker = new Timer(new TimerCallback(DoNet), null, TickTime, Timeout.Infinite);
            tickTimer = new Timer(new TimerCallback(DoTick), null, TickTime, Timeout.Infinite);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <remarks></remarks>
        public void Stop()
        {
            try
            {
                netListener.Stop();
            }
            catch { }
            finally
            {
                if (worker != null)
                    worker.Dispose();
                worker = null;
                netListener = null;

#if DEBUG
                if (LogEnabled)
                    logFile.Close();
#endif
            }
        }

        /// <summary>
        /// Internal SendChat method, includes event
        /// handling
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="c">The c.</param>
        /// <remarks></remarks>
        protected internal virtual void _SendChat(string Message, RemoteClient c)
        {
            ChatEventArgs e = new ChatEventArgs(Message);
            e.RemoteClient = c;
            if (OnChat != null)
                OnChat(this, e);
            if (e.Handled)
                return;
            lock (SendQueueLock)
            {
                foreach (RemoteClient client in GetLoggedInClients())
                    client.PacketQueue.Enqueue(new ChatMessagePacket(Message));
            }
        }

        /// <summary>
        /// Sends a chat message to all connected players.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <remarks></remarks>
        public virtual void SendChat(string Message)
        {
            lock (SendQueueLock)
            {
                foreach (RemoteClient client in GetLoggedInClients())
                    client.PacketQueue.Enqueue(new ChatMessagePacket(Message));
            }
        }

        /// <summary>
        /// Sends the player to given dimension.
        /// </summary>
        /// <param name="c">The client to send.</param>
        /// <param name="d">The target dimension.</param>
        /// <remarks></remarks>
        public virtual void SendPlayerToDimension(RemoteClient c, Dimension d)
        {
            if (c.PlayerEntity.Dimension == d)
                return;
            c.PlayerEntity.LoadedColumns.Clear();
            c.PlayerEntity.Dimension = d;
            c.PacketQueue.Enqueue(new RespawnPacket(c, GetWorld(c)));
            c.PacketQueue.Enqueue(new PlayerPositionAndLookPacket(c.PlayerEntity));
            ChunkManager.RecalculateClientColumns(c, this, true);
        }

        /// <summary>
        /// Kicks the player.
        /// </summary>
        /// <param name="Name">The player name.</param>
        /// <param name="Reason">The reason.</param>
        /// <remarks></remarks>
        public virtual void KickPlayer(string Name, string Reason)
        {
            RemoteClient r = ConnectedClients.Where(c => c.LoggedIn && c.PlayerEntity.Name.ToLower() == Name.ToLower()).First();
            r.PacketQueue.Clear();
            r.PacketQueue.Enqueue(new DisconnectPacket(Reason));
        }

        /// <summary>
        /// Creates the lightning.
        /// </summary>
        /// <param name="Location">The location to spawn lightning.</param>
        /// <remarks></remarks>
        public virtual void CreateLightning(Vector3 Location)
        {
            EnqueueToAllClients(new ThunderboltPacket(NextEntityID, Location));
        }

        /// <summary>
        /// Changes the player mode.
        /// </summary>
        /// <param name="Name">The name of the player to update.</param>
        /// <param name="Mode">The mode to assign.</param>
        /// <remarks></remarks>
        public virtual void ChangePlayerMode(string Name, GameMode Mode)
        {
            RemoteClient r = ConnectedClients.Where(c => c.LoggedIn && c.PlayerEntity.Name.ToLower() == Name.ToLower()).First();
            r.PacketQueue.Enqueue(new NewOrInvalidStatePacket(NewOrInvalidState.ChangeGameMode, Mode == GameMode.Creative));
        }

        /// <summary>
        /// Sends the chat to player.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="Chat">The chat.</param>
        /// <remarks></remarks>
        public virtual void SendChatToPlayer(string Name, string Chat)
        {
            RemoteClient r = ConnectedClients.Where(c => c.LoggedIn && c.PlayerEntity.Name.ToLower() == Name.ToLower()).First();
            r.PacketQueue.Enqueue(new ChatMessagePacket(Chat));
        }

        /// <summary>
        /// Sets the weather in a specific world.
        /// </summary>
        /// <param name="On">If the weather is on</param>
        /// <param name="World">The world to set the weather in.</param>
        public virtual void SetWeatherInWorld(bool On, World World)
        {
            // TODO
            foreach (RemoteClient r in GetClientsInWorld(World))
            {
                r.PacketQueue.Enqueue(new NewOrInvalidStatePacket(On ? NewOrInvalidState.BeginRain : NewOrInvalidState.EndRain));
            }
        }

        /// <summary>
        /// Shows the credits to a given player.
        /// </summary>
        /// <param name="PlayerName">Name of the player.</param>
        /// <remarks></remarks>
        public virtual void ShowCredits(string PlayerName)
        {
            RemoteClient r = ConnectedClients.Where(c => c.LoggedIn && c.PlayerEntity.Name.ToLower() == PlayerName.ToLower()).First();
            r.PacketQueue.Enqueue(new NewOrInvalidStatePacket(NewOrInvalidState.EnterCredits));
        }

        /// <summary>
        /// Adds the specified level to the server.
        /// </summary>
        /// <param name="world">The level to add.</param>
        /// <remarks></remarks>
        public virtual void AddLevel(Level level)
        {
            Levels.Add(level);
            level.OnBlockChange += new EventHandler<BlockChangeEventArgs>(level_OnBlockChange);
            level.PropertyChanged += new PropertyChangedEventHandler(level_PropertyChanged);
        }

        #endregion

        #region Worker

        /// <summary>
        /// 
        /// </summary>
        int ticksPassed = 0;
        /// <summary>
        /// 
        /// </summary>
        int ticksSinceSave = 0;
        /// <summary>
        /// 
        /// </summary>
        internal KeepAlivePacket keepAlive;

        /// <summary>
        /// If overriden, please call base.DoTick
        /// to ensure Keep-Alives are sent.
        /// </summary>
        /// <remarks></remarks>
        protected virtual void DoTick(object o)
        {
            lock (SendQueueLock)
            {
                ticksPassed++;
                if (ticksPassed == 50)
                {
                    // Send Keep-Alive
                    keepAlive = new KeepAlivePacket();
                    foreach (RemoteClient c in GetLoggedInClients())
                    {
                        c.PacketQueue.Enqueue(keepAlive);
                        foreach (RemoteClient e in GetClientsInWorldExcept(GetWorld(c), c))
                        {
                            c.PacketQueue.Enqueue(new EntityTeleportPacket(e.PlayerEntity.ID, e.PlayerEntity.Location, e.PlayerEntity.Rotation));
                        }
                    }
                    ticksPassed = 0;
                }
            }
            //Tick the scheduled update thing
            ScheduledUpdateManager.Tick();
            ticksSinceSave++;
            foreach (Level l in Levels)
            {
                l._Time++; // TODO: Further updates FINISHED
            }
            if (ticksSinceSave == SaveFrequency)
                ticksSinceSave = 0;
            if (OnTick != null)
                OnTick(this, new EventArgs());
            tickTimer = new Timer(new TimerCallback(DoTick), null, TickTime, Timeout.Infinite);
        }

        /// <summary>
        /// Does the net.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <remarks></remarks>
        private void DoNet(object o)
        {
            try
            {
                // Handle incoming connections
                if (netListener.Pending())
                {
                    RemoteClient client = new RemoteClient(netListener.AcceptTcpClient());
                    client.TcpClient.NoDelay = true;
                    client.TcpClient.SendBufferSize = 40960;
                    ConnectedClients.Add(client);
                    Log("New connection from " + client.TcpClient.Client.RemoteEndPoint.ToString());
                }
                if (ConnectedClients.Count == 0)
                    return;
                // Read incoming data
                List<RemoteClient> remainingClients = new List<RemoteClient>();
                foreach (RemoteClient client in ConnectedClients)
                {
                    if (client.TcpClient.Connected) // failsafe
                        remainingClients.Add(client);
                    else
                    {
                        Log("Lost connection from " + client.LoginEndPoint.ToString());
                        if (client.LoggedIn)
                        {
                            EnqueueToAllClients(new PlayerListItemPacket(client.PlayerEntity.Name, false, 0));
                            foreach (RemoteClient c in GetClientsInWorldExcept(GetWorld(client), client))
                                c.PacketQueue.Enqueue(new DestroyEntityPacket(client.PlayerEntity.ID));
                            GetWorld(client).RemoveEntity(client.PlayerEntity.ID);
                            if (OnPlayerLeave != null)
                                OnPlayerLeave(this, new PlayerEventArgs(client));
                        }
                        continue;
                    }

                    while (client.TcpClient.Connected && client.TcpClient.Available != 0)
                    {
                        PacketID packetID = (PacketID)client.TcpClient.GetStream().ReadByte();
                        Packet packet = Packet.GetPacket(packetID);
                        if (PrePacket != null)
                        {
                            PacketEventArgs e = new PacketEventArgs(packetID, client, PacketMode.Recieving, packet);
                            PrePacket(this, e);
                            if (e.Handled)
                                continue;
                        }
                        if (packet.GetType() != typeof(InvalidPacket))
                        {
                            packet.ReadPacket(client);
                            packet.HandlePacket(this, client);
                            client.LogPacket(packet);
                            LogPacket(packet, client, true);

                            // Special packet event handlers
                            if (packet is LoginRequestPacket && client.LoggedIn)
                            {
                                client.PlayerEntity.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PlayerEntity_PropertyChanged);
                                if (OnPlayerJoin != null)
                                    OnPlayerJoin(this, new PlayerEventArgs(client));
                            }
                            if (packet is DisconnectPacket)
                            {
                                Log("Lost connection from " + client.LoginEndPoint.ToString());
                                if (client.LoggedIn)
                                {
                                    EnqueueToAllClientsExcept(new PlayerListItemPacket(client.PlayerEntity.Name, false, 0), client.PlayerEntity);
                                    foreach (RemoteClient c in GetClientsInWorldExcept(GetWorld(client), client))
                                        c.PacketQueue.Enqueue(new DestroyEntityPacket(client.PlayerEntity.ID));
                                    GetWorld(client).RemoveEntity(client.PlayerEntity.ID);
                                    if (OnPlayerLeave != null)
                                        OnPlayerLeave(this, new PlayerEventArgs(client));
                                    client.TcpClient.Close();
                                    if (remainingClients.Contains(client))
                                        remainingClients.Remove(client);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Log("Client sent unimplemented packet!  Expect crashes and errors! (" + packetID.ToString() + ")");
                            if (client.TcpClient.Available != 0)
                                client.TcpClient.GetStream().Read(new byte[client.TcpClient.Available], 0, client.TcpClient.Available);
                            KickPlayer(client.PlayerEntity.Name, "Unimplemented packet sent (" + packetID.ToString() + ")");
                        }
                        if (PostPacket != null)
                        {
                            PacketEventArgs e = new PacketEventArgs(packetID, client, PacketMode.Recieving, packet);
                            PostPacket(this, e);
                        }
                    }
                }
                ConnectedClients = remainingClients;

                foreach (RemoteClient client in ConnectedClients)
                {
                    // Do individual client updates
                    if (client.LoggedIn)
                    {
                        if (client.PlayerEntity.MapLoadRadiusDueTime != -1)
                            client.PlayerEntity.MapLoadRadiusDueTime--;
                        if (client.PlayerEntity.MapLoadRadiusDueTime == -1)
                        {
                            if (client.PlayerEntity.MapLoadRadius != 5)
                            {
                                client.PlayerEntity.MapLoadRadius++;
                                ChunkManager.RecalculateClientColumns(client, this, true);
                                client.PlayerEntity.MapLoadRadiusDueTime = 50;
                            }
                        }
                        ChunkManager.RecalculateClientColumns(client, this);
                    }

                    // Write pending data
                    lock (SendQueueLock)
                    {
                        while (client.PacketQueue.Count != 0) // TODO: Spread large amounts of packets over several ticks
                        {
                            Packet packet = client.PacketQueue.Dequeue();
                            if (LogPackets)
                                LogPacket(packet, client, false);
                            if (PrePacket != null)
                            {
                                PacketEventArgs e = new PacketEventArgs(packet.PacketID, client, PacketMode.Sending, packet);
                                e.Payload = packet.Payload;
                                PrePacket(this, e);
                                if (e.Handled)
                                    continue;
                            }
                            try
                            {
                                client.TcpClient.GetStream().Write(packet.Payload, 0, packet.Payload.Length);
                            }
                            catch (InvalidOperationException e)
                            {
                                Log(e.ToString());
                                client.PacketQueue.Clear();
                                break;
                            }
                            catch (IOException e)
                            {
                                Log(e.ToString());
                                client.PacketQueue.Clear();
                                break;
                            }
                            catch (Exception e)
                            {
                                Log(e.ToString());
                            }
                            if (packet.PacketID == PacketID.Disconnect)
                            {
                                //OnDisconnect
                                client.TcpClient.Close();
                            }
                            if (PostPacket != null)
                            {
                                PacketEventArgs e = new PacketEventArgs(packet.PacketID, client, PacketMode.Sending, packet);
                                e.Payload = packet.Payload;
                                PostPacket(this, e);
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                worker = new Timer(new TimerCallback(DoNet), null, TickTime, Timeout.Infinite);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the client object for a player of the given name.
        /// </summary>
        /// <param name="Name">The name of the player to find.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public RemoteClient GetClient(string Name)
        {
            var i = from c in GetLoggedInClients()
                    where c.PlayerEntity.Name.ToLower() == Name.ToLower()
                    select c;
            if (i.Count() == 0)
                return null;
            return i.First();
        }

        /// <summary>
        /// Gets the client object for the given player entity.
        /// </summary>
        /// <param name="Player">The player entity to find.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public RemoteClient GetClient(PlayerEntity Player)
        {
            var i = from c in GetLoggedInClients()
                    where c.PlayerEntity.ID == Player.ID
                    select c;
            if (i.Count() == 0)
                return null;
            return i.First();
        }

        /// <summary>
        /// Gets the remote clients in a world.
        /// </summary>
        /// <param name="w">The world.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerable<RemoteClient> GetClientsInWorld(World w)
        {
            return from c in GetLoggedInClients()
                   where w.Entities.Contains(c.PlayerEntity)
                   select c;
        }

        
        /// <summary>
        /// Gets all RemoteClients in a given world, except for the specified client.
        /// </summary>
        /// <param name="w">The world.</param>
        /// <param name="Except">The client to exclude.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerable<RemoteClient> GetClientsInWorldExcept(World w, RemoteClient Except)
        {
            return from c in GetLoggedInClients()
                   where w.Entities.Contains(c.PlayerEntity) 
                   && c.PlayerEntity.Name.ToLower() != Except.PlayerEntity.Name.ToLower()
                   select c;
        }


        public IEnumerable<RemoteClient> GetClientsInLevel(Level l)
        {
             List<RemoteClient> clients =  new List<RemoteClient>();

             clients.AddRange(GetClientsInWorld(l.TheEnd));
             clients.AddRange(GetClientsInWorld(l.Overworld));
             clients.AddRange(GetClientsInWorld(l.Nether));

             return from c in GetLoggedInClients()
                    where clients.Contains(c)
                    select c;
        }


        /// <summary>
        /// Gets the world a given remote client is currently in.
        /// </summary>
        /// <param name="c">The remote client.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public World GetWorld(RemoteClient c)
        {
            return Levels[c.PlayerEntity.LevelIndex].GetWorld(c.PlayerEntity.Dimension);
        }

        /// <summary>
        /// Gets the world a given entity is currently in.
        /// </summary>
        /// <param name="e">The entity.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public World GetWorld(Entity e)
        {
            return Levels[e.LevelIndex].GetWorld(e.Dimension);
        }

        /// <summary>
        /// Gets the logged in remote clients.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<RemoteClient> GetLoggedInClients()
        {
            return new List<RemoteClient>(from c in ConnectedClients
                                          where c.LoggedIn
                                          select c);
        }

        /// <summary>
        /// Enqueues the given packet to all logged in clients.
        /// </summary>
        /// <param name="b">The packet to enqueue.</param>
        /// <remarks></remarks>
        public void EnqueueToAllClients(Packet b)
        {
            lock (SendQueueLock)
            {
                foreach (RemoteClient c in GetLoggedInClients())
                    c.PacketQueue.Enqueue(b);
            }
        }

        /// <summary>
        /// Enqueues a given packet to all clients, except for the specified player.
        /// </summary>
        /// <param name="b">The packet to enqueue.</param>
        /// <param name="player">This player will not recieve the packet.</param>
        /// <remarks></remarks>
        public void EnqueueToAllClientsExcept(Packet b, PlayerEntity player)
        {
            lock (SendQueueLock)
            {
                foreach (RemoteClient client in from c in GetLoggedInClients()
                                                where c.PlayerEntity.Name != player.Name
                                                select c)
                    client.PacketQueue.Enqueue(b);
            }
        }

        [Conditional("DEBUG")]
        private void LogPacket(Packet p, RemoteClient c, bool fromClient)
        {
            if (LogPackets)
            {
                if (fromClient)
                    Log("[CLIENT->SERVER " + c.LoginEndPoint.ToString() + "]: " + p.PacketID.ToString() + " (" + p.Payload.Length + " byte" + (p.Payload.Length != 1 ? "s" : "") + ")");
                else
                    Log("[SERVER->CLIENT " + c.LoginEndPoint.ToString() + "]: " + p.PacketID.ToString() + " (" + p.Payload.Length + " byte" + (p.Payload.Length != 1 ? "s" : "") + ")");
                Log("\t{" + MultiplayerClient.DumpArray(p.Payload) + "}");
                Log("\tValues:");
                foreach (var prop in p.GetType().GetProperties().Where(property => property.Name != "Payload" && property.Name != "PacketID" && property.Name != "Length"))
                {
                    if (prop.PropertyType == typeof(byte[]))
                    {
                        Log("\t\t" + prop.Name + ": {" + MultiplayerClient.DumpArray((byte[])prop.GetValue(p, null)) + "}");
                    }
                    else
                        Log("\t\t" + prop.Name + ": " + prop.GetValue(p, null));
                }
            }
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <remarks></remarks>
        [Conditional("DEBUG")]
        public void Log(string Message)
        {
            if (!LogEnabled)
                return;
            if (Message.Length < 250)
                Console.WriteLine(Message);
            try
            {
                logFile.WriteLine("{" + DateTime.Now.ToLongTimeString() + "}: " + Message);
                logFile.Flush();
            }
            catch { }
        }

        #endregion

        #region Event Handlers

        void level_OnBlockChange(object sender, BlockChangeEventArgs e)
        {
            foreach (RemoteClient r in GetClientsInWorld(sender as World)) // FIX THIS
                r.PacketQueue.Enqueue(new BlockChangePacket(e.Position, e.Block));
        }

        void level_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Level level = (sender as Level);
            switch (e.PropertyName)
            {
                case "Time":
                    foreach (RemoteClient r in GetClientsInLevel(level))
                    {
                        r.PacketQueue.Enqueue(new TimeUpdatePacket((level.Time)));
                    }


                    break;
            }
        }


        /// <summary>
        /// Handles the PropertyChanged event of the PlayerEntity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        void PlayerEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RemoteClient client = GetClient((sender as PlayerEntity).Name);
            switch (e.PropertyName)
            {
                case "Health":
                case "Food":
                case "FoodSaturation":
                    break;
                case "Stance":
                case "OnGround":
                case "Location":
                case "Rotation":
                    break;
                case "HeadRotation":
                    EnqueueToAllClientsExcept(new EntityHeadLookPacket(client.PlayerEntity.ID,
                        client.PlayerEntity.HeadRotation), client.PlayerEntity);
                    break;
                case "Dimension":
                    client.PacketQueue.Enqueue(new RespawnPacket(client, GetWorld(client)));
                    client.PlayerEntity.LoadedColumns.Clear();
                    ChunkManager.RecalculateClientColumns(client, this, true);
                    client.PacketQueue.Enqueue(new PlayerPositionAndLookPacket(client.PlayerEntity));
                    break;
                case "GameMode":
                    client.PacketQueue.Enqueue(new NewOrInvalidStatePacket(NewOrInvalidState.ChangeGameMode,
                        client.PlayerEntity.GameMode == GameMode.Creative));
                    break;
                case "Inventory":
                    break;
            }
        }

        #endregion
    }
}
