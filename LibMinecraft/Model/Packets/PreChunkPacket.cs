using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// This class contains information about the prechunks to be sent to the client
    /// </summary>
    /// <remarks></remarks>
    public class PreChunkPacket : Packet
    {
        /// <summary>
        /// Gets or sets the chunk X.
        /// </summary>
        /// <value>The chunk X.</value>
        /// <remarks></remarks>
        public int ChunkX { get; set; }
        /// <summary>
        /// Gets or sets the chunk Z.
        /// </summary>
        /// <value>The chunk Z.</value>
        /// <remarks></remarks>
        public int ChunkZ { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PreChunkPacket"/> is unload.
        /// </summary>
        /// <value><c>true</c> if unload; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Unload { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreChunkPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PreChunkPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreChunkPacket"/> class.
        /// </summary>
        /// <param name="ChunkX">The chunk X.</param>
        /// <param name="ChunkZ">The chunk Z.</param>
        /// <remarks></remarks>
        public PreChunkPacket(double ChunkX, double ChunkZ)
        {
            this.ChunkX = (int)(ChunkX / 16);
            this.ChunkZ = (int)(ChunkZ / 16);
            this.Unload = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreChunkPacket"/> class.
        /// </summary>
        /// <param name="ChunkX">The chunk X.</param>
        /// <param name="ChunkZ">The chunk Z.</param>
        /// <param name="Unload">if set to <c>true</c> [unload].</param>
        /// <remarks></remarks>
        public PreChunkPacket(double ChunkX, double ChunkZ, bool Unload)
        {
            this.ChunkX = (int)(ChunkX / 16);
            this.ChunkZ = (int)(ChunkZ / 16);
            this.Unload = Unload;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.PreChunk; }
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
                return new byte[] { (byte)PacketID.PreChunk }
                    .Concat(MakeInt(ChunkX))
                    .Concat(MakeInt(ChunkZ))
                    .Concat(MakeBoolean(!Unload)).ToArray();
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
