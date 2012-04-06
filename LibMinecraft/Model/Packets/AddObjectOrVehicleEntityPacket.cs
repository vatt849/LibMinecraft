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
    public class AddObjectOrVehicleEntityPacket : Packet
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        /// <remarks></remarks>
        public int EntityID { get; set; }
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        /// <remarks></remarks>
        public byte EntityType { get; set; }
        /// <summary>
        /// Gets or sets the entity location.
        /// </summary>
        /// <value>The entity location.</value>
        /// <remarks></remarks>
        public Vector3 EntityLocation { get; set; }
        /// <summary>
        /// Gets or sets the fireball origin ID.
        /// </summary>
        /// <value>The fireball origin ID.</value>
        /// <remarks></remarks>
        public int FireballOriginID { get; set; }
        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        /// <value>The velocity.</value>
        /// <remarks></remarks>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddObjectOrVehicleEntityPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public AddObjectOrVehicleEntityPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddObjectOrVehicleEntityPacket"/> class.
        /// </summary>
        /// <param name="EntityID">The entity ID.</param>
        /// <param name="EntityType">Type of the entity.</param>
        /// <param name="EntityLocation">The entity location.</param>
        /// <remarks></remarks>
        public AddObjectOrVehicleEntityPacket(int EntityID, byte EntityType, Vector3 EntityLocation)
        {
            this.EntityID = EntityID;
            this.EntityType = EntityType;
            this.EntityLocation = EntityLocation;
            this.FireballOriginID = 0;
            this.Velocity = Vector3.Zero;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.AddObjectOrVehicle; }
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
                if (FireballOriginID == 0)
                {
                    return new byte[] { (byte)PacketID }
                        .Concat(MakeInt(EntityID))
                        .Concat(new byte[] { EntityType })
                        .Concat(MakeAbsoluteInt(EntityLocation.X))
                        .Concat(MakeAbsoluteInt(EntityLocation.Y))
                        .Concat(MakeAbsoluteInt(EntityLocation.Z))
                        .Concat(MakeInt(FireballOriginID)).ToArray();
                }

                return new byte[] { (byte)PacketID }
                        .Concat(MakeInt(EntityID))
                        .Concat(new byte[] { EntityType })
                        .Concat(MakeAbsoluteInt(EntityLocation.X))
                        .Concat(MakeAbsoluteInt(EntityLocation.Y))
                        .Concat(MakeAbsoluteInt(EntityLocation.Z))
                        .Concat(MakeInt(FireballOriginID))
                        .Concat(MakeShort((short)Velocity.X))
                        .Concat(MakeShort((short)Velocity.Y))
                        .Concat(MakeShort((short)Velocity.Z)).ToArray();
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
