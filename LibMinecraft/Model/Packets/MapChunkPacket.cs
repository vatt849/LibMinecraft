using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class MapChunkPacket : Packet
    {
        public Vector3 Location { get; set; }
        public bool GroundUpContinuous { get; set; }
        public ushort PrimaryBitMap { get; set; }
        public ushort AddBitMap { get; set; }
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the chunk.
        /// </summary>
        /// <value>The chunk.</value>
        /// <remarks></remarks>
        public Chunk Chunk { get; set; }

        public byte[] BiomeData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapChunkPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MapChunkPacket()
        {
            Location = Vector3.Zero;
            this.GroundUpContinuous = false;
            this.PrimaryBitMap = 0;
            this.AddBitMap = 0;
            this.Data = new byte[0];
            this.BiomeData = new byte[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapChunkPacket"/> class.
        /// </summary>
        /// <param name="PrimaryBitMap">A bit map representing which chunks are contained in this packet</param>
        /// <param name="Location">The location of this chunk</param>
        /// <param name="GroundUpContiguous">True if this column contains chunks spanning the map's entire vertical height</param>
        /// <param name="Data">Block data to use for this chunk column</param>
        /// <param name="BiomeData">Biomes to (optionally) specify in this chunk column</param>
        /// <param name="AddBitMap">Optional amount to add to this chunk column's data</param>
        /// <remarks></remarks>
        public MapChunkPacket(Vector3 Location, bool GroundUpContiguous, ushort PrimaryBitMap,
            ushort AddBitMap, byte[] Data, byte[] BiomeData)
        {
            this.Location = Location;
            this.GroundUpContinuous = GroundUpContinuous;
            this.PrimaryBitMap = PrimaryBitMap;
            this.AddBitMap = AddBitMap;
            this.Data = Data;
            this.BiomeData = BiomeData;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.MapChunk; }
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
                return new byte[] { (byte)PacketID.MapChunk }
                    .Concat(MakeInt((int)(Location.X / 16)))
                    .Concat(MakeInt((int)(Location.Z / 16)))
                    .Concat(MakeBoolean(GroundUpContinuous))
                    .Concat(MakeUShort(PrimaryBitMap))
                    .Concat(MakeUShort(AddBitMap))
                    .Concat(MakeInt(Data.Length))
                    .Concat(MakeInt(0))
                    .Concat(Data).ToArray();
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
