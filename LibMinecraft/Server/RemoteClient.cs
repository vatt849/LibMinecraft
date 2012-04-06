using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using LibMinecraft.Model.Entities;
using System.Net;
using LibMinecraft.Model;
using LibMinecraft.Model.Packets;

namespace LibMinecraft.Server
{
    /// <summary>
    /// Represents a Minecraft client currently connected
    /// to the server.
    /// </summary>
    /// <remarks></remarks>
    public class RemoteClient
    {
        /// <summary>
        /// The connection to this client
        /// </summary>
        /// <value>The TCP client.</value>
        /// <remarks></remarks>
        public TcpClient TcpClient { get; set; }
        // TODO: Change to Queue<Packet>
        /// <summary>
        /// Packets waiting to be sent to the client
        /// </summary>
        /// <value>The packet queue.</value>
        /// <remarks></remarks>
        public Queue<Packet> PacketQueue { get; set; }
        /// <summary>
        /// Whether or not this player has logged in yet
        /// </summary>
        /// <value><c>true</c> if [logged in]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool LoggedIn { get; set; }
        /// <summary>
        /// The entity this player is represented by
        /// </summary>
        /// <value>The player entity.</value>
        /// <remarks></remarks>
        public PlayerEntity PlayerEntity { get; set; }
        /// <summary>
        /// The client's end point at the time a connection was established
        /// </summary>
        /// <value>The login end point.</value>
        /// <remarks></remarks>
        public EndPoint LoginEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the online mode hash.
        /// </summary>
        /// <value>The online mode hash.</value>
        /// <remarks></remarks>
        internal string OnlineModeHash { get; set; }
        /// <summary>
        /// Keeps a record of the past 10 packets recieved by a client.
        /// </summary>
        /// <value>The packet record.</value>
        /// <remarks></remarks>
        public Queue<PacketRecord> PacketRecord { get; set; }

        /// <summary>
        /// Place any value you please in these.
        /// </summary>
        /// <value>The tags.</value>
        /// <remarks></remarks>
        public Dictionary<string, object> Tags { get; set; }

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        /// <value>The hostname.</value>
        /// <remarks></remarks>
        public string Hostname { get; set; }

        /// <summary>
        /// Logs the packet.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <remarks></remarks>
        internal void LogPacket(Packet p)
        {
            if (PacketRecord == null)
                PacketRecord = new Queue<PacketRecord>();
            if (PacketRecord.Count >= 9)
                PacketRecord.Dequeue();
            PacketRecord.Enqueue(new PacketRecord(p, DateTime.Now));
            PacketRecord.TrimExcess();
        }

        /// <summary>
        /// Default constructor for RemoteClient
        /// </summary>
        /// <remarks></remarks>
        public RemoteClient()
        {
            Tags = new Dictionary<string, object>();
            PacketRecord = new Queue<PacketRecord>(10);
        }

        /// <summary>
        /// Creates a new RemoteClient based on the given TcpClient
        /// </summary>
        /// <param name="client">The client.</param>
        /// <remarks></remarks>
        public RemoteClient(TcpClient client)
        {
            this.TcpClient = client;
            PacketQueue = new Queue<Packet>();
            LoggedIn = false;
            LoginEndPoint = client.Client.RemoteEndPoint;
            Tags = new Dictionary<string, object>();
        }

        /// <summary>
        /// Sends the chat to the remote client.
        /// </summary>
        /// <param name="Message">The message to send.</param>
        /// <remarks></remarks>
        public void SendChat(string Message)
        {
            PacketQueue.Enqueue(new ChatMessagePacket(Message));
        }
    }
}
