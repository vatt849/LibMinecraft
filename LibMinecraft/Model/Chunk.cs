using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibMinecraft.Server;


namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a chunk of blocks.
    /// </summary>
    /// <remarks></remarks>
    public class Chunk
    {
        /// <summary>
        /// A static deflater for chunk compression.
        /// </summary>
        public static Deflater zlib = new Deflater();
        /// <summary>
        /// Gets or sets the blocks.
        /// </summary>
        /// <value>The blocks.</value>
        /// <remarks></remarks>
        public byte[] Blocks { get; set; }
        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        /// <remarks></remarks>
        public byte[] Metadata { get; set; } // TODO: Nibble array?
        /// <summary>
        /// Gets or sets the sky light.
        /// </summary>
        /// <value>The sky light.</value>
        /// <remarks></remarks>
        public byte[] SkyLight { get; set; }
        /// <summary>
        /// Gets or sets the block light.
        /// </summary>
        /// <value>The block light.</value>
        /// <remarks></remarks>
        public byte[] BlockLight { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        /// <remarks>This location is the forward-top-left corner of a chunk.</remarks>
        public Vector3 Location { get; set; }

        /// <summary>
        /// The width (x) of a chunk.
        /// </summary>
        public const byte Width = 16;
        /// <summary>
        /// The depth (z) of a chunk.
        /// </summary>
        public const byte Depth = 16;
        /// <summary>
        /// The height (y) of a chunk.
        /// </summary>
        public const byte Height = 16;

        public MapColumn Column { get; set; }

        /// <summary>
        /// Gets or sets the additional block data data.
        /// </summary>
        /// <value>The additional data.</value>
        /// <remarks></remarks>
        public Dictionary<Vector3, byte[]> AdditionalData { get; set; }

        public bool IsAir { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Chunk(Vector3 Location)
        {
            this.Location = Location;
            IsAir = true;
            Blocks = new byte[Width * Depth * Height];
            Metadata = new byte[Width * Depth * Height];
            BlockLight = new byte[Width * Depth * Height];
            SkyLight = new byte[Width * Depth * Height];
            for (int i = 0; i < BlockLight.Length; i++)
            {
                BlockLight[i] = 0xF;
                SkyLight[i] = 0xF;
            }
            if (AdditionalData == null)
                AdditionalData = new Dictionary<Vector3, byte[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk"/> class.
        /// </summary>
        /// <param name="world">The column to initialize to.</param>
        /// <remarks></remarks>
        public Chunk(MapColumn column, Vector3 Location) : this(Location)
        {
            this.Column = column;
        }

        public virtual byte[] GetMetadata()
        {
            byte[] metadata = new byte[Metadata.Length / 2];
            for (int i = 0; i < Metadata.Length; i += 2)
                metadata[i / 2] = (byte)(Metadata[i] + (Metadata[i + 1] << 4));
            return metadata;
        }

        public virtual byte[] GetBlockLight()
        {
            byte[] blockLight = new byte[BlockLight.Length / 2];
            for (int i = 0; i < BlockLight.Length; i += 2)
                blockLight[i / 2] = (byte)(BlockLight[i] + (BlockLight[i + 1] << 4));
            return blockLight;
        }

        public virtual byte[] GetSkyLight()
        {
            byte[] skyLight = new byte[SkyLight.Length / 2];
            for (int i = 0; i < SkyLight.Length; i += 2)
                skyLight[i / 2] = (byte)(SkyLight[i] + (SkyLight[i + 1] << 4));
            return skyLight;
        }

        /// <summary>
        /// Gets the uncompressed chunk data, with blocks, metadata,
        /// and lighting concatenated into an array.
        /// </summary>
        /// <returns>An uncompressed array of raw chunk data.</returns>
        /// <remarks>Note that this does not contain Chunk.AdditionalData.</remarks>
        public virtual byte[] GetData()
        {
            byte[] data = new byte[12544];

            // Blocks
            Array.Copy(Blocks, data, Blocks.Length);

            // Metadata
            // TODO: Make more efficient
            int blockIndex = 0;
            int i = Blocks.Length;
            for (; i < 6144; i++)
            {
                byte b = (byte)(Metadata[blockIndex] + (byte)(Metadata[blockIndex + 1] << 4));
                data[i] = b;
                blockIndex += 2;
            }

            // Light
            for (; i < 8192; i++)
                data[i] = 0x00;
            for (; i < 10240; i++)
                data[i] = 0xFF;
            // Add
            for (; i < 12288; i++)
                data[i] = 0x0;
            // Biome
            for (i = 0; i < 256; i++)
                data[i] = 0x0;

            return data;
        }
        /// <summary>
        /// Compresses the specified data using zlib compression.
        /// </summary>
        /// <param name="compress">The data.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected virtual byte[] Compress(byte[] compress)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (DeflaterOutputStream stream = new DeflaterOutputStream(memory, zlib))
                    stream.Write(compress, 0, compress.Length);
                compress = memory.ToArray();
            }
            zlib.Reset();
            return compress;
        }

        /// <summary>
        /// Gets a block from this chunk.
        /// </summary>
        /// <param name="pos">The position, in local chunk coordinates.</param>
        /// <returns>The specified block.</returns>
        /// <remarks></remarks>
        public virtual Block GetBlock(Vector3 pos)
        {
            if (IsAir)
                return new AirBlock();
            int x = (int)pos.X;
            int y = (int)pos.Y;
            int z = (int)pos.Z;
            int index = x + (z * Width) + (y * Height * Width);
            Block b = Blocks[index];
            b.Metadata = Metadata[index];
            if (AdditionalData.ContainsKey(pos.Floor()))
                b.AdditionalData.Deseralize(AdditionalData[pos.Floor()].Skip(1).ToArray());
            return b;
        }

        /// <summary>
        /// Sets a block in this chunk.
        /// </summary>
        /// <param name="pos">The position in local coordinates.</param>
        /// <param name="block">The block to set.</param>
        /// <remarks></remarks>
        public virtual void SetBlock(Vector3 pos, Block block)
        {
            int x = (int)pos.X;
            int y = (int)pos.Y;
            int z = (int)pos.Z;
            int index = x + (z * Width) + (y * Height * Width);
            Metadata[index] = block.Metadata;
            Blocks[index] = block;
            if (block.AdditionalData == null)
            {
                if (AdditionalData.ContainsKey(pos.Floor()))
                    AdditionalData.Remove(pos.Floor());
            }
            if (block.AdditionalData != null)
                AdditionalData[pos.Floor()] = new byte[] {
                    block.AdditionalData.BlockID }.Concat(block.AdditionalData.Serialize()).ToArray();

            if (!(block is AirBlock))
                IsAir = false;
            else
            {
                IsAir = true;
                for (int i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i] != 0)
                    {
                        IsAir = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Compresses a full-byte metadata array into a
        /// half-byte metadata array and saves it to this chunk.
        /// </summary>
        /// <param name="MetadataArray">The full-byte metadata array.</param>
        /// <remarks></remarks>
        protected internal virtual void SetMetadata(byte[] MetadataArray)
        {
            Metadata = new byte[Width * Depth * Height];
            for (int i = 0; i < MetadataArray.Length; i++)
            {
                Metadata[i / 2] = (byte)(MetadataArray[i] & 0xF0);
                Metadata[i / 2 + 1] = (byte)(MetadataArray[i] & 0x0F << 8);
            }
        }

        /// <summary>
        /// Sets the block light.
        /// </summary>
        /// <param name="LightArray">The light array.</param>
        /// <remarks></remarks>
        protected internal virtual void SetBlockLight(byte[] LightArray)
        {
            BlockLight = new byte[Width * Depth * Height];
            for (int i = 0; i < LightArray.Length; i++)
            {
                BlockLight[i / 2] = (byte)(LightArray[i] & 0xF0);
                BlockLight[i / 2 + 1] = (byte)(LightArray[i] & 0x0F << 8);
            }
        }

        /// <summary>
        /// Sets the sky light.
        /// </summary>
        /// <param name="LightArray">The light array.</param>
        /// <remarks></remarks>
        protected internal virtual void SetSkyLight(byte[] LightArray)
        {
            SkyLight = new byte[Width * Depth * Height];
            for (int i = 0; i < LightArray.Length; i++)
            {
                SkyLight[i / 2] = (byte)(LightArray[i] & 0xF0);
                SkyLight[i / 2 + 1] = (byte)(LightArray[i] & 0x0F << 8);
            }
        }
    }
}
