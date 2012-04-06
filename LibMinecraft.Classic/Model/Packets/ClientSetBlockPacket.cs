using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Classic.Server;
using LibMinecraft.Classic.Client;

namespace LibMinecraft.Classic.Model.Packets
{
    /// <summary>
    /// Packet sent to client when block is placed.
    /// </summary>
    /// <remarks></remarks>
    public class ClientSetBlockPacket : Packet
    {
        /// <summary>
        /// Gets or sets the position of the block.
        /// </summary>
        /// <value>The position.</value>
        /// <remarks></remarks>
        public Vector3 Position { get; set; }
        /// <summary>
        /// Gets or sets the mode (1 or 0).
        /// </summary>
        /// <value>The mode. 1 = place, 0 = destroy</value>
        /// <remarks></remarks>
        public byte Mode { get; set; }
        /// <summary>
        /// Gets or sets the block type.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks></remarks>
        public byte Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSetBlockPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ClientSetBlockPacket()
        {
            Position = new Vector3();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSetBlockPacket"/> class.
        /// </summary>
        /// <param name="Position">The position.</param>
        /// <param name="Mode">The mode.</param>
        /// <param name="Type">The type.</param>
        /// <remarks></remarks>
        public ClientSetBlockPacket(Vector3 Position, byte Mode, byte Type)
        {
            this.Position = Position;
            this.Mode = Mode;
            this.Type = Type;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.ClientSetBlock; }
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
                return new byte[] { (byte)PacketID }
                    .Concat(MakeDouble(Position.X)).ToArray()
                    .Concat(MakeDouble(Position.Y)).ToArray()
                    .Concat(MakeDouble(Position.Z)).ToArray()
                    .Concat(new byte[] { Mode, Type }).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            Position.X = ReadShort(Client.TcpClient.GetStream());
            Position.Y = ReadShort(Client.TcpClient.GetStream());
            Position.Z = ReadShort(Client.TcpClient.GetStream());
            this.Mode = (byte)Client.TcpClient.GetStream().ReadByte();
            this.Type = (byte)Client.TcpClient.GetStream().ReadByte();
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
            World.Find(Client.World)._SetBlock(Position, Type);
            //Send the block to all clients.
            if (Mode != 0)
                Server.EnqueueToAllClients(new ServerSetBlockPacket(Position, Type));
            else
                Server.EnqueueToAllClients(new ServerSetBlockPacket(Position, 0));
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
