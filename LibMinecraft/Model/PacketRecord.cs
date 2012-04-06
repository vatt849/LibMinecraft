using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Packets;

namespace LibMinecraft.Model
{
    /// <summary>
    /// A record of a packet sent by a client.
    /// </summary>
    public class PacketRecord
    {
        /// <summary>
        /// Time this packet was recieved
        /// </summary>
        public DateTime Recieved { get; set; }
        /// <summary>
        /// The packet sent by the client
        /// </summary>
        public Packet Packet { get; set; }

        /// <summary>
        /// Creates a new packet record
        /// </summary>
        public PacketRecord(Packet Packet, DateTime Recieved)
        {
            this.Packet = Packet;
            this.Recieved = Recieved;
        }
    }
}
