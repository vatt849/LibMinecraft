using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class HeldItemChangePacket : Packet
    {
        /// <summary>
        /// Gets or sets the index of the slot.
        /// </summary>
        /// <value>The index of the slot.</value>
        /// <remarks></remarks>
        public short SlotIndex { get; set; }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.HeldItemChange; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 3; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID }.Concat(MakeShort(SlotIndex)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            SlotIndex = ReadShort(Client.TcpClient.GetStream());
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
            throw new InvalidOperationException();
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
            Client.PlayerEntity.SelectedSlot = (short)(PlayerEntity.HotbarOffset + SlotIndex);
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }
    }
}
