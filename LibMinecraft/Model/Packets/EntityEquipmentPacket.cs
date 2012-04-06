using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Client;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class EntityEquipmentPacket : Packet
    {
        /// <summary>
        /// Gets or sets the Entity ID
        /// </summary>
        /// <value>The entity ID</value>
        public int EntityID { get; set; }
        /// <summary>
        /// Gets or sets the index of the slot.
        /// </summary>
        /// <value>The index of the slot.</value>
        /// <remarks></remarks>
        public short SlotIndex { get; set; }
        /// <summary>
        /// The item ID held, or worn
        /// </summary>
        /// <value>An Item ID</value>
        public short ItemID { get; set; }
        /// <summary>
        /// The damage value of the item
        /// </summary>
        /// <value>Damage value</value>
        public short Damage { get; set; }
        /// <summary>
        /// Gets the packet ID
        /// </summary>
        public override PacketID PacketID
        {
            get { return PacketID.EntityEquipment; }
        }
        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 11; }
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
                    .Concat(MakeShort(SlotIndex))
                    .Concat(MakeShort(ItemID))
                    .Concat(MakeShort(Damage)).ToArray();
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
            throw new InvalidOperationException();
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
