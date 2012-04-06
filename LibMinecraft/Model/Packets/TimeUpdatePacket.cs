using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Client;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// The packets for time updates
    /// </summary>
    /// <remarks></remarks>
    public class TimeUpdatePacket : Packet
    {
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        /// <remarks></remarks>
        public long Time { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeUpdatePacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public TimeUpdatePacket()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeUpdatePacket"/> class.
        /// </summary>
        /// <param name="Time">The time.</param>
        /// <remarks></remarks>
        public TimeUpdatePacket(long Time)
        {
            this.Time = Time;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.TimeUpdate; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 9; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID }.Concat(MakeLong(Time)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(RemoteClient Client)
        {
            Client.TcpClient.GetStream().Write(Payload, 0, Length);
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(Client.MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
