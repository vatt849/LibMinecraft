using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Classic.Server;
using LibMinecraft.Classic.Client;

namespace LibMinecraft.Classic.Model.Packets
{
    /// <summary>
    /// Packet sent to client when sending the world.
    /// </summary>
    /// <remarks></remarks>
    public class WorldDataChunkPacket : Packet
    {
        /// <summary>
        /// Gets or sets the length of the chunk.
        /// </summary>
        /// <value>The length of the chunk.</value>
        /// <remarks></remarks>
        public short ChunkLength { get; set; }
        /// <summary>
        /// Gets or sets the chunk data.
        /// </summary>
        /// <value>The chunk data.</value>
        /// <remarks></remarks>
        public byte[] ChunkData { get; set; }
        /// <summary>
        /// Gets or sets the percentage complete.
        /// </summary>
        /// <value>The percentage complete.</value>
        /// <remarks></remarks>
        public byte PercentageComplete { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldDataChunkPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public WorldDataChunkPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldDataChunkPacket"/> class.
        /// </summary>
        /// <param name="ChunkLength">Length of the chunk.</param>
        /// <param name="ChunkData">The chunk data.</param>
        /// <param name="PercentageComplete">The percentage complete.</param>
        /// <remarks></remarks>
        public WorldDataChunkPacket(short ChunkLength, byte[] ChunkData, byte PercentageComplete)
        {
            this.ChunkLength = ChunkLength;
            this.ChunkData = ChunkData;
            this.PercentageComplete = PercentageComplete;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.LevelDataChunk; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 1028; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID, }
                    .Concat(MakeShort(ChunkLength)).ToArray()
                    .Concat(ChunkData).ToArray()
                    .Concat(new byte[] { PercentageComplete }).ToArray();
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
