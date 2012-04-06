using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model;
using LibMinecraft.Model.Packets;

namespace LibMinecraft.Server
{
    /// <summary>
    /// The packet modes
    /// </summary>
    /// <remarks></remarks>
    public enum PacketMode
    {
        /// <summary>
        /// Recieving the packet (incoming)
        /// </summary>
        Recieving,
        /// <summary>
        /// Sending the packet (Outgoing)
        /// </summary>
        Sending,
    }

    /// <summary>
    /// Used to describe the state of a
    /// transaction in progress.
    /// </summary>
    /// <remarks></remarks>
    public class PacketEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the packet.
        /// </summary>
        /// <value>The packet.</value>
        /// <remarks></remarks>
        public Packet Packet { get; set; }
        /// <summary>
        /// Gets or sets the packet ID.
        /// </summary>
        /// <value>The packet ID.</value>
        /// <remarks></remarks>
        public PacketID PacketID { get; set; }
        /// <summary>
        /// Gets or sets the remote client.
        /// </summary>
        /// <value>The remote client.</value>
        /// <remarks></remarks>
        public RemoteClient RemoteClient { get; set; }
        /// <summary>
        /// Gets or sets the packet mode.
        /// </summary>
        /// <value>The packet mode.</value>
        /// <remarks></remarks>
        public PacketMode PacketMode { get; set; }
        /// <summary>
        /// Only set for outbound packets
        /// </summary>
        /// <value>The payload.</value>
        /// <remarks></remarks>
        public byte[] Payload { get; set; }
        /// <summary>
        /// Set to true if you do not want the packet processed by LibMinecraft.
        /// If you do this, you must use Packet.ReadPacket to ensure that packet
        /// syncronization is not interrupted.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketEventArgs"/> class.
        /// </summary>
        /// <param name="PacketID">The packet ID.</param>
        /// <param name="RemoteClient">The remote client.</param>
        /// <param name="PacketMode">The packet mode.</param>
        /// <param name="Packet">The packet.</param>
        /// <remarks></remarks>
        public PacketEventArgs(PacketID PacketID, RemoteClient RemoteClient, PacketMode PacketMode, Packet Packet)
        {
            this.PacketID = PacketID;
            this.RemoteClient = RemoteClient;
            this.Handled = false;
            this.Packet = Packet;
        }
    }
}
