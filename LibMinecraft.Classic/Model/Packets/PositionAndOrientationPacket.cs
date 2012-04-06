using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Classic.Server;
using LibMinecraft.Classic.Client;

namespace LibMinecraft.Classic.Model.Packets
{
    /// <summary>
    /// Packet sent to server/client when teleporting/moving. This is the most inefficent packet if only movement or rotation has changed.
    /// </summary>
    /// <remarks></remarks>
    public class PositionAndOrientationPacket : Packet
    {
        /// <summary>
        /// Gets or sets the player ID.
        /// </summary>
        /// <value>The player ID.</value>
        /// <remarks></remarks>
        public byte PlayerID { get; set; }
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
        /// Initializes a new instance of the <see cref="PositionAndOrientationPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PositionAndOrientationPacket()
        {
            Position = new Vector3();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionAndOrientationPacket"/> class.
        /// </summary>
        /// <param name="PlayerID">The player ID.</param>
        /// <param name="Position">The position.</param>
        /// <param name="Yaw">The yaw.</param>
        /// <param name="Pitch">The pitch.</param>
        /// <remarks></remarks>
        public PositionAndOrientationPacket(byte PlayerID, Vector3 Position, byte Yaw, byte Pitch)
        {
            this.PlayerID = PlayerID;
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
            get { return PacketID.PositionAndOrientation; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 10; }
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
            this.PlayerID = (byte)Client.TcpClient.GetStream().ReadByte();
            this.Position.X = ReadShort(Client.TcpClient.GetStream());
            this.Position.Y = ReadShort(Client.TcpClient.GetStream());
            this.Position.Z = ReadShort(Client.TcpClient.GetStream());
            this.Yaw = (byte)Client.TcpClient.GetStream().ReadByte();
            this.Pitch = (byte)Client.TcpClient.GetStream().ReadByte();
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
            Client.Position = Position;
            Position.X = Position.X / 32;
            Position.Y = Position.Y / 32;
            Position.Z = Position.Z / 32;
            Client.Yaw = Yaw;
            Client.Pitch = Pitch;
            Server.EnqueueToAllClientsInWorld(new PositionAndOrientationPacket(Client.PlayerID, Position, Yaw, Pitch), Client.World, Client);
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
