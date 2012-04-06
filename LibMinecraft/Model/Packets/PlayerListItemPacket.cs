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
    public class PlayerListItemPacket : Packet
    {
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        /// <value>The name of the player.</value>
        /// <remarks></remarks>
        public string PlayerName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PlayerListItemPacket"/> is online.
        /// </summary>
        /// <value><c>true</c> if online; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Online { get; set; }
        /// <summary>
        /// Gets or sets the ping.
        /// </summary>
        /// <value>The ping.</value>
        /// <remarks></remarks>
        public short Ping { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerListItemPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PlayerListItemPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerListItemPacket"/> class.
        /// </summary>
        /// <param name="PlayerName">Name of the player.</param>
        /// <param name="Online">if set to <c>true</c> [online].</param>
        /// <param name="Ping">The ping.</param>
        /// <remarks></remarks>
        public PlayerListItemPacket(string PlayerName, bool Online, short Ping)
        {
            this.PlayerName = PlayerName;
            this.Online = Online;
            this.Ping = Ping;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.PlayerListItem; }
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
                    .Concat(MakeString(PlayerName))
                    .Concat(MakeBoolean(Online))
                    .Concat(MakeShort(Ping)).ToArray();
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
