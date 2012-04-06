using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Client;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// The packet for a thunderbolt stike
    /// </summary>
    /// <remarks></remarks>
    public class ThunderboltPacket : Packet
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        /// <remarks></remarks>
        public int EntityID { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ThunderboltPacket"/> is unknown.
        /// </summary> 
        /// <value><c>true</c> if unknown; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Unknown { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        /// <remarks></remarks>
        public Vector3 Location { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThunderboltPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ThunderboltPacket()
        {
            Unknown = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThunderboltPacket"/> class.
        /// </summary>
        /// <param name="EntityID">The entity ID.</param>
        /// <param name="Location">The location.</param>
        /// <remarks></remarks>
        public ThunderboltPacket(int EntityID, Vector3 Location)
        {
            this.Unknown = true;
            this.EntityID = EntityID;
            this.Location = Location;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.Thunderbolt; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 18; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] 
                {
                    (byte)PacketID
                }.Concat(MakeInt(EntityID))
                .Concat(MakeBoolean(Unknown))
                .Concat(MakeAbsoluteInt(Location.X))
                .Concat(MakeAbsoluteInt(Location.Y))
                .Concat(MakeAbsoluteInt(Location.Z)).ToArray();
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
            throw new NotImplementedException();
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
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
