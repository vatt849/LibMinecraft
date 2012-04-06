﻿using System;
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
    public class EntityRelativeMovePacket : Packet
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        /// <remarks></remarks>
        public int EntityID { get; set; }
        /// <summary>
        /// Gets or sets the delta X.
        /// </summary>
        /// <value>The delta X.</value>
        /// <remarks></remarks>
        public sbyte DeltaX { get; set; }
        /// <summary>
        /// Gets or sets the delta Y.
        /// </summary>
        /// <value>The delta Y.</value>
        /// <remarks></remarks>
        public sbyte DeltaY { get; set; }
        /// <summary>
        /// Gets or sets the delta Z.
        /// </summary>
        /// <value>The delta Z.</value>
        /// <remarks></remarks>
        public sbyte DeltaZ { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRelativeMovePacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public EntityRelativeMovePacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRelativeMovePacket"/> class.
        /// </summary>
        /// <param name="EntityID">The entity ID.</param>
        /// <param name="OldLocation">The old location.</param>
        /// <param name="NewLocation">The new location.</param>
        /// <remarks></remarks>
        public EntityRelativeMovePacket(int EntityID, Vector3 OldLocation, Vector3 NewLocation)
        {
            this.EntityID = EntityID;
            this.DeltaX = (sbyte)((NewLocation.X - OldLocation.X) * 32.0);
            this.DeltaY = (sbyte)((NewLocation.Y - OldLocation.Y) * 32.0);
            this.DeltaZ = (sbyte)((NewLocation.Z - OldLocation.Z) * 32.0);
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.EntityRelativeMove; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 8; }
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
                    .Concat(new byte[]
                    {
                        (byte)DeltaX,
                        (byte)DeltaY,
                        (byte)DeltaZ,
                    }).ToArray();
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
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
