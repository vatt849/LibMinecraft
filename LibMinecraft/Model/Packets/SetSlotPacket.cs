using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// The packet to set the slot of a window.
    /// </summary>
    /// <remarks></remarks>
    public class SetSlotPacket : Packet
    {
        /// <summary>
        /// Gets or sets the window ID.
        /// </summary>
        /// <value>The window ID.</value>
        /// <remarks></remarks>
        public byte WindowID { get; set; }
        /// <summary>
        /// Gets or sets the index of the slot.
        /// </summary>
        /// <value>The index of the slot.</value>
        /// <remarks></remarks>
        public short SlotIndex { get; set; }
        /// <summary>
        /// Gets or sets the slot data.
        /// </summary>
        /// <value>The slot data.</value>
        /// <remarks></remarks>
        public Slot SlotData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetSlotPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public SetSlotPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetSlotPacket"/> class.
        /// </summary>
        /// <param name="WindowID">The window ID.</param>
        /// <param name="SlotIndex">Index of the slot.</param>
        /// <param name="SlotData">The slot data.</param>
        /// <remarks></remarks>
        public SetSlotPacket(byte WindowID, short SlotIndex, Slot SlotData)
        {
            this.WindowID = WindowID;
            this.SlotIndex = SlotIndex;
            this.SlotData = SlotData;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.SetSlot; }
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
                return new byte[] { (byte)PacketID, WindowID }
                    .Concat(MakeShort(SlotIndex))
                    .Concat(SlotData.GetData()).ToArray();
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
