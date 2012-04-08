using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using LibMinecraft.Client;
using System.Xml.Serialization;
using System.ComponentModel;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a Minecraft server
    /// </summary>
    /// <remarks></remarks>
    public class MinecraftServer// : INotifyPropertyChanged TODO
    {
        #region Properties

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>The hostname.</value>
        /// <remarks></remarks>
        [XmlIgnore]
        public string Hostname
        {
            get
            {
                return IP + ":" + Port.ToString();
            }
            set
            {
                if (value.StartsWith("http://"))
                    value = value.Substring("http://".Length);
                if (value.Contains(":"))
                {
                    Port = int.Parse(value.Substring(value.IndexOf(":") + 1));
                    value = value.Remove(value.IndexOf(":"));
                }
                IP = value;
            }
        }

        /// <summary>
        /// Gets or sets the IP.
        /// </summary>
        /// <value>The IP.</value>
        /// <remarks></remarks>
        public string IP { get; set; }
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        /// <remarks></remarks>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the mot D.
        /// </summary>
        /// <value>The mot D.</value>
        /// <remarks></remarks>
        public string MotD { get; set; }
        /// <summary>
        /// The number of players currently connected
        /// </summary>
        /// <value>The connected players.</value>
        /// <remarks></remarks>
        [XmlIgnore]
        public int ConnectedPlayers { get; set; }
        /// <summary>
        /// Gets or sets the max players.
        /// </summary>
        /// <value>The max players.</value>
        /// <remarks></remarks>
        public int MaxPlayers { get; set; }
        /// <summary>
        /// Time required to ping server (in milliseconds)
        /// </summary>
        /// <value>The ping time.</value>
        /// <remarks></remarks>
        [XmlIgnore]
        public int PingTime { get; set; }

        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        /// <value>The difficulty.</value>
        /// <remarks></remarks>
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [online mode].
        /// </summary>
        /// <value><c>true</c> if [online mode]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool OnlineMode { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        /// <remarks></remarks>
        public double Time { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MinecraftServer"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MinecraftServer()
        {
            this.MotD = "";
            this.ConnectedPlayers = 0;
            this.MaxPlayers = 20;
            this.Port = 25565;
            this.IP = "127.0.0.1";
        }

        #endregion

        #region GetServer
        /// <summary>
        /// Gets server status based on the hostname.
        /// The hostname may include a port, like
        /// "127.0.0.1:port"
        /// </summary>
        /// <param name="Hostname">The hostname.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static MinecraftServer GetServer(string Hostname)
        {
            MinecraftServer server = new MinecraftServer();
            int port = 25565;

            if (Hostname.StartsWith("http://"))
                Hostname = Hostname.Substring("http://".Length);
            if (Hostname.Contains(":"))
            {
                port = int.Parse(Hostname.Substring(Hostname.IndexOf(":") + 1));
                Hostname = Hostname.Remove(Hostname.IndexOf(":"));
            }
            server.Port = port;
            server.IP = Hostname;

            byte[] resp;

            DateTime startTime = DateTime.Now;
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(Hostname, port);
                socket.Send(new byte[] { (byte)PacketID.ServerListPing });
                resp = new byte[socket.ReceiveBufferSize];
                socket.Receive(resp);
                socket.Disconnect(false);
            }
            catch { return server; }
            TimeSpan ping = DateTime.Now - startTime;
            server.PingTime = (int)ping.TotalMilliseconds;

            if (resp[0] != (byte)PacketID.Disconnect) // Used to acknowledge
                return server;

            string Description = "";
            string numPlayers = "";
            string maxPlayers = "";
            // Read out description
            int i;
            for (i = 4; i < resp.Length; i += 2)
            {
                if (resp[i] == 0xA7) // End of string
                    break;
                Description += (char)resp[i];
            }
            i += 2;
            // Read out number of players
            for (; i < resp.Length; i += 2)
            {
                if (resp[i] == 0xA7) // End of string
                    break;
                numPlayers += (char)resp[i];
            }
            i += 2;
            // Read out max players
            for (; i < resp.Length; i += 2)
            {
                if (resp[i] == 0x00) // End of data
                    break;
                maxPlayers += (char)resp[i];
            }
            server.MotD = Description;
            server.MaxPlayers = int.Parse(maxPlayers);
            server.ConnectedPlayers = int.Parse(numPlayers);

            return server;
        }

        /// <summary>
        /// Gets server status based on the hostname.
        /// The hostname may include a port, like
        /// "127.0.0.1:port"
        /// </summary>
        /// <param name="Hostname">The hostname.</param>
        /// <param name="AsyncCallback">The async callback.</param>
        /// <param name="AsyncState">State of the async.</param>
        /// <remarks></remarks>
        public static void GetServerAsync(string Hostname, Action<MinecraftServer, object> AsyncCallback, object AsyncState)
        {
            ParameterizedThreadStart start = new ParameterizedThreadStart(GetServerWorker);
            Thread thread = new Thread(start);
            thread.Start(new GetServerWorkerData()
                {
                    AsyncCallback = AsyncCallback,
                    AsyncState = AsyncState,
                    Hostname = Hostname,
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks></remarks>
        struct GetServerWorkerData
        {
            /// <summary>
            /// 
            /// </summary>
            public string Hostname;
            /// <summary>
            /// 
            /// </summary>
            public Action<MinecraftServer, object> AsyncCallback;
            /// <summary>
            /// 
            /// </summary>
            public object AsyncState;
        }

        /// <summary>
        /// Gets the server worker.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <remarks></remarks>
        static void GetServerWorker(object input)
        {
            GetServerWorkerData data = (GetServerWorkerData)input;
            MinecraftServer response = GetServer(data.Hostname);
            if (data.AsyncCallback != null)
                data.AsyncCallback(response, data.AsyncState);
        }
        #endregion
    }
}
