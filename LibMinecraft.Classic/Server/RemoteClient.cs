using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using LibMinecraft.Classic.Model;
using LibMinecraft.Classic.Model.Packets;

namespace LibMinecraft.Classic.Server
{
    /// <summary>
    /// The Remote Client
    /// </summary>
    /// <remarks></remarks>
    public class RemoteClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteClient"/> class.
        /// </summary>
        /// <remarks></remarks>
        public RemoteClient()
        {
            this.PacketQueue = new Queue<Packet>();
            this.LoggedIn = false;
            this.UserType = Model.UserType.Normal;
        }

        /// <summary>
        /// Gets or sets the TCP client.
        /// </summary>
        /// <value>The TCP client.</value>
        /// <remarks></remarks>
        public TcpClient TcpClient { get; set; }
        /// <summary>
        /// Gets or sets the end point (IP Address).
        /// </summary>
        /// <value>The end point.</value>
        /// <remarks></remarks>
        public EndPoint EndPoint { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        /// <remarks></remarks>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the player ID.
        /// </summary>
        /// <value>The player ID.</value>
        /// <remarks></remarks>
        public byte PlayerID { get; set; }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        /// <remarks></remarks>
        public Vector3 Position { get; set; }
        /// <summary>
        /// Gets or sets the world.
        /// </summary>
        /// <value>The world.</value>
        /// <remarks></remarks>
        public string World { get; set; }
        /// <summary>
        /// Gets or sets the yaw.
        /// </summary>
        /// <value>The yaw.</value>
        /// <remarks></remarks>
        public byte Yaw { get; set; }
        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
        /// <remarks></remarks>
        public byte Pitch { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether logged in.
        /// </summary>
        /// <value><c>true</c> if logged in; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool LoggedIn { get; set; }
        /// <summary>
        /// Gets or sets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        /// <remarks></remarks>
        public UserType UserType { get; set; }
        /// <summary>
        /// Gets or sets the packet queue.
        /// </summary>
        /// <value>The packet queue.</value>
        /// <remarks></remarks>
        public Queue<Packet> PacketQueue { get; set; }
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        /// <remarks></remarks>
        public Dictionary<string, object> Tags { get; set; }
    }
}
