using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using LibMinecraft.Classic.Model;
using System.Security.Cryptography;
using LibMinecraft.Classic.Model.Packets;

namespace LibMinecraft.Classic.Server
{
    /// <summary>
    /// Runs the classic network protocol as a server.
    /// </summary>
    /// <remarks></remarks>
    public class ClassicServer
    {
        #region Properties

        /// <summary>
        /// Gets the server URL.
        /// </summary>
        /// <value>The server URL.</value>
        /// <remarks></remarks>
        public string ServerUrl { get; internal set; }

        /// <summary>
        /// Gets or sets the server model.
        /// </summary>
        /// <value>The server.</value>
        /// <remarks></remarks>
        public MinecraftClassicServer Server { get; set; }

        /// <summary>
        /// A list of all connected clients.
        /// </summary>
        /// <value>The clients.</value>
        /// <remarks></remarks>
        public List<RemoteClient> Clients { get; set; }

        /// <summary>
        /// A list of all the worlds.
        /// </summary>
        /// <value>The worlds.</value>
        /// <remarks></remarks>
        public static List<World> Worlds { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// The server listener.
        /// </summary>
        private TcpListener listener;
        /// <summary>
        /// The main server tick.
        /// </summary>
        private Timer netWorker;
        /// <summary>
        /// The heartbeat tick.
        /// </summary>
        private Timer heartBeat;
        /// <summary>
        /// The server salt.
        /// </summary>
        private string ServerSalt;
        /// <summary>
        /// The main level of the server.
        /// </summary>
        public string MainLevel;
        /// <summary>
        /// The world directory.
        /// </summary>
        public string WorldDirectory;

        #region Constants

        /// <summary>
        /// The Minecraft Version (used in heartbeat)
        /// </summary>
        private const int MinecraftVersion = 7;
        /// <summary>
        /// The heartbeat request URL.
        /// </summary>
        private const string HeartbeatRequestUrl = "http://minecraft.net/heartbeat.jsp?port={0}&max={1}&name={2}&public={3}&version={4}&salt={5}&users={6}";

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the players connection state changes.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<PlayerConnectionEventArgs> OnPlayerConnectionChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the server with the provided server model.
        /// </summary>
        /// <param name="Server">The server model to represent this server.</param>
        /// <param name="WorldDirectory">The world directory.</param>
        /// <param name="WorldName">Name of the world.</param>
        /// <returns>The URL to use for connecting to this server via minecraft.net</returns>
        /// <remarks>This listens on all available network interfaces.</remarks>
        public string Start(MinecraftClassicServer Server, string WorldDirectory = "levels", string WorldName = "main")
        {
            this.Server = Server;
            ServerSalt = "";
            Clients = new List<RemoteClient>();

            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), Server.Port); // Listen on all interfaces
            listener = new TcpListener(localEP);
            listener.Start();

            Worlds = new List<World>();
            MainLevel = WorldName;
            this.WorldDirectory = WorldDirectory;
            if (!Directory.Exists(WorldDirectory + "/"))
                Directory.CreateDirectory(WorldDirectory + "/");

            Worlds.Add(new World(MainLevel, 128, 128, 128, new Vector3(64, 128 + 2, 64)));
            try { Worlds[0].LoadFromBinary(WorldDirectory, MainLevel + ".lvl"); } catch { }
            
            DoHeartbeat(null);

            heartBeat = new Timer(new TimerCallback(DoHeartbeat), null, 45000, 45000); // Send heartbeat every 45 seconds
            netWorker = new Timer(new TimerCallback(DoWork), null, 0, Timeout.Infinite);

            return ServerUrl;
        }

        /// <summary>
        /// Starts the server with the provided server model,
        /// on the specified local end point.
        /// </summary>
        /// <param name="Server">The server model to represent this server.</param>
        /// <param name="LocalEndPoint">The local end point.</param>
        /// <returns>The URL to use for connecting to this server via minecraft.net</returns>
        /// <remarks></remarks>
        public string Start(MinecraftClassicServer Server, IPEndPoint LocalEndPoint)
        {
            this.Server = Server;
            ServerSalt = "";
            Clients = new List<RemoteClient>();

            listener = new TcpListener(LocalEndPoint);
            listener.Start();

            DoHeartbeat(null);

            heartBeat = new Timer(new TimerCallback(DoHeartbeat), null, 45000, 45000); // Send heartbeat every 45 seconds
            netWorker = new Timer(new TimerCallback(DoWork), null, 0, Timeout.Infinite);

            return ServerUrl;
        }

        #endregion

        #region Worker

        /// <summary>
        /// Does the heartbeat.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <remarks></remarks>
        protected void DoHeartbeat(object o)
        {
            // Generate a salt
            lock (ServerSalt)
            {
                ServerSalt = "";

                // Generate a secure salt
                RNGCryptoServiceProvider rcsp = new RNGCryptoServiceProvider();
                byte[] saltData = new byte[16];
                rcsp.GetNonZeroBytes(saltData);
                ServerSalt = Encoding.UTF8.GetString(saltData);

                string reqString = string.Format(HeartbeatRequestUrl, Server.Port, Server.MaxPlayers,
                    Uri.EscapeDataString(Server.Name), Server.Private ? "False" : "True",
                    MinecraftVersion, "", Clients.Count);

                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqString);
                    WebResponse resp = req.GetResponse();
                    StreamReader reader = new StreamReader(resp.GetResponseStream());
                    string response = reader.ReadToEnd();
                    ServerUrl = response;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Does the work.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <remarks></remarks>
        protected void DoWork(object o)
        {
            if (listener.Pending())
            {
                RemoteClient c = new RemoteClient();
                c.TcpClient = listener.AcceptTcpClient();
                c.EndPoint = c.TcpClient.Client.RemoteEndPoint;
                Clients.Add(c);
            }

            int totalQueuedPackets = 0;
            for (int i = 0; i < Clients.Count; i++)
                totalQueuedPackets += Clients[i].PacketQueue.Count;
            // Any leftover time from each client is given to any client that could use it
            double additionalTime = 0;

            for (int i = 0; i < Clients.Count; i++)
            {
                if (!Clients[i].TcpClient.Connected)
                {
                    if (OnPlayerConnectionChanged != null && Clients[i].LoggedIn)
                        OnPlayerConnectionChanged(this, new PlayerConnectionEventArgs(ConnectionState.Disconnected, Clients[i]));
                    EnqueueToAllClientsInWorld(new DespawnPlayerPacket((byte)i), Clients[i].World);
                }
                else
                {
                    try
                    {
                        // Read in a packet
                        if (Clients[i].TcpClient.Available != 0)
                        {
                            Packet p = Packet.GetPacket((PacketID)Clients[i].TcpClient.GetStream().ReadByte(), true);
                            p.ReadPacket(Clients[i]);
                            p.HandlePacket(this, Clients[i]);
                            if (p.PacketID == PacketID.Identification && OnPlayerConnectionChanged != null)
                                OnPlayerConnectionChanged(this, new PlayerConnectionEventArgs(ConnectionState.Connected, Clients[i]));
                        }
                        // Write out from the packet queue
                        if (Clients[i].PacketQueue.Count == 0)
                            continue;
                        // For the sake of server speed, each client is limited in the amount
                        // of time per tick they are allocated for sending of packets.
                        DateTime startTime = DateTime.Now;
                        double maxClientSendTime = double.PositiveInfinity;

                        if (!(totalQueuedPackets == 0 || Clients[i].PacketQueue.Count / totalQueuedPackets == 0))
                            maxClientSendTime = (1 / 20) * (Clients[i].PacketQueue.Count / totalQueuedPackets) + additionalTime;

                        additionalTime = 0;
                        while (Clients[i].PacketQueue.Count != 0 && (DateTime.Now - startTime).TotalMilliseconds <= maxClientSendTime)
                        {
                            // Send out packets from the queue
                            PacketID id = Clients[i].PacketQueue.Peek().PacketID;
                            Clients[i].PacketQueue.Dequeue().WritePacket(Clients[i]);
                            if (id == PacketID.DisconnectPlayer)
                            {
                                if (Clients[i].TcpClient.Connected)
                                    Clients[i].TcpClient.Close();
                                if (OnPlayerConnectionChanged != null)
                                    OnPlayerConnectionChanged(this, new PlayerConnectionEventArgs(ConnectionState.Disconnected, Clients[i]));
                                EnqueueToAllClientsInWorld(new DespawnPlayerPacket((byte)i), Clients[i].World); 
                                Clients.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                        if ((DateTime.Now - startTime).TotalMilliseconds <= maxClientSendTime)
                            additionalTime += maxClientSendTime - (DateTime.Now - startTime).TotalMilliseconds;
                    }
                    catch
                    {
                        if (OnPlayerConnectionChanged != null && Clients[i].LoggedIn)
                            OnPlayerConnectionChanged(this, new PlayerConnectionEventArgs(ConnectionState.Disconnected, Clients[i]));
                        EnqueueToAllClients(new DespawnPlayerPacket((byte)i), Clients[i]); //TODO: Find out why this exceptions.   
                        Clients.RemoveAt(i);
                        i--;
                    }
                }
            }

            netWorker = new Timer(new TimerCallback(DoWork), null, 0, Timeout.Infinite);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Enqueues a packet to all clients.
        /// </summary>
        /// <param name="Packet">The packet.</param>
        /// <param name="IgnoreClient">If specified, the client won't recieve the packet.</param>
        /// <remarks></remarks>
        public void EnqueueToAllClients(Packet Packet, RemoteClient IgnoreClient = null)
        {
            foreach (RemoteClient Client in Clients)
                if (IgnoreClient != Client)
                    Client.PacketQueue.Enqueue(Packet);
        }

        /// <summary>
        /// Enqueues to all clients in a certain world.
        /// </summary>
        /// <param name="Packet">The packet.</param>
        /// <param name="World">The world.</param>
        /// <param name="IgnoreClient">If specified, the client won't recieve the packet.</param>
        /// <remarks></remarks>
        public void EnqueueToAllClientsInWorld(Packet Packet, string World, RemoteClient IgnoreClient = null)
        {
            foreach (RemoteClient Client in Clients)
                if (IgnoreClient != Client && Client.World.ToLower() == World.ToLower())
                    Client.PacketQueue.Enqueue(Packet);
        }

        /// <summary>
        /// Loads all players to client.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public void LoadAllClientsToClient(RemoteClient Client)
        {
            foreach (RemoteClient others in Clients)
                if (others != Client)
                    Client.PacketQueue.Enqueue(new SpawnPlayerPacket(others.PlayerID,
                        others.UserName, others.Position, others.Yaw, others.Pitch));
        }
        #endregion
    }
}
