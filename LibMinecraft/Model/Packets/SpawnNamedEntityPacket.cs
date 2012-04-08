using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class SpawnNamedEntityPacket : Packet
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        /// <remarks></remarks>
        public int EntityID { get; set; }
        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>The name of the entity.</value>
        /// <remarks></remarks>
        public string EntityName { get; set; }
        /// <summary>
        /// Gets or sets the entity location.
        /// </summary>
        /// <value>The entity location.</value>
        /// <remarks></remarks>
        public Vector3 EntityLocation { get; set; }
        /// <summary>
        /// Gets or sets the entity rotation.
        /// </summary>
        /// <value>The entity rotation.</value>
        /// <remarks></remarks>
        public Vector3 EntityRotation { get; set; }
        /// <summary>
        /// Gets or sets the held item.
        /// </summary>
        /// <value>The held item.</value>
        /// <remarks></remarks>
        public short HeldItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnNamedEntityPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public SpawnNamedEntityPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnNamedEntityPacket"/> class.
        /// </summary>
        /// <param name="EntityID">The entity ID.</param>
        /// <param name="EntityName">Name of the entity.</param>
        /// <param name="EntityLocation">The entity location.</param>
        /// <param name="EntityRotation">The entity rotation.</param>
        /// <param name="HeldItem">The held item.</param>
        /// <remarks></remarks>
        public SpawnNamedEntityPacket(int EntityID, string EntityName, Vector3 EntityLocation,
            Vector3 EntityRotation, short HeldItem)
        {
            this.EntityID = EntityID;
            this.EntityName = EntityName;
            this.EntityLocation = EntityLocation;
            this.EntityRotation = EntityRotation;
            this.HeldItem = HeldItem;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.SpawnNamedEntity; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return -1; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID }
                    .Concat(MakeInt(EntityID))
                    .Concat(MakeString(EntityName))
                    .Concat(MakeAbsoluteInt(EntityLocation.X))
                    .Concat(MakeAbsoluteInt(EntityLocation.Y))
                    .Concat(MakeAbsoluteInt(EntityLocation.Z))
                    .Concat(new byte[]
                        {
                            MakePackedByte((float)EntityRotation.X),
                            MakePackedByte((float)EntityRotation.Y),
                        })
                    .Concat(MakeShort(HeldItem)).ToArray();
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
            Client.TcpClient.GetStream().Write(Payload, 0, Payload.Length);
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
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
