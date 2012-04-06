using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Classic.Server;
using LibMinecraft.Classic.Client;

namespace LibMinecraft.Classic.Model.Packets
{
    /// <summary>
    /// Packet sent to server when spawning. If player hasn't despawned, this simply creates another player, and the other player is a "ghost".
    /// </summary>
    /// <remarks></remarks>
    public class SpawnPlayerPacket : Packet
    {
        /// <summary>
        /// Gets or sets the player ID.
        /// </summary>
        /// <value>The player ID.</value>
        /// <remarks></remarks>
        public byte PlayerID { get; set; }
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        /// <value>The name of the player.</value>
        /// <remarks></remarks>
        public string PlayerName { get; set; }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        /// <remarks></remarks>
        public Vector3 Position { get; set; }
        /// <summary>
        /// Gets or sets the yaw.
        /// </summary>
        /// <value>The yaw.</value>
        /// <remarks></remarks>
        public byte Yaw { get; set; }
        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
        /// <remarks></remarks>
        public byte Pitch { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnPlayerPacket"/> class.
        /// </summary>
        /// <param name="PlayerID">The player ID.</param>
        /// <param name="PlayerName">Name of the player.</param>
        /// <param name="Position">The position.</param>
        /// <param name="Yaw">The yaw.</param>
        /// <param name="Pitch">The pitch.</param>
        /// <remarks></remarks>
        public SpawnPlayerPacket(byte PlayerID, string PlayerName, Vector3 Position, byte Yaw, byte Pitch)
        {
            this.PlayerID = PlayerID;
            this.PlayerName = PlayerName;
            this.Position = Position;
            this.Yaw = Yaw;
            this.Pitch = Pitch;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.SpawnPlayer; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 74; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID, PlayerID }
                    .Concat(MakeString(PlayerName)).ToArray()
                    .Concat(MakeDouble(Position.X)).ToArray()
                    .Concat(MakeDouble(Position.Y)).ToArray()
                    .Concat(MakeDouble(Position.Z)).ToArray()
                    .Concat(new byte[] { Yaw, Pitch }).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(ClassicClient Client)
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
        public override void WritePacket(ClassicClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(ClassicServer Server, RemoteClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(ClassicClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
