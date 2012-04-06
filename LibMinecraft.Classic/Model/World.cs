using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// Represents a Minecraft Classic level.
    /// </summary>
    /// <remarks></remarks>
    public class World
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <param name="WorldName">Name of the world.</param>
        /// <param name="Height">The height.</param>
        /// <param name="Width">The width.</param>
        /// <param name="Depth">The depth.</param>
        /// <param name="SpawnLocation">The spawn location.</param>
        /// <remarks></remarks>
        public World(string WorldName, short Height, short Width, short Depth, Vector3 SpawnLocation)
        {
            this.Height = Height;
            this.Width = Width;
            this.Depth = Depth;
            this.WorldName = WorldName;
            Tags = new Dictionary<string, string>();
            Data = new byte[Width * Depth * Height];
            if (WorldName == "main") //Will get overridden if main.lvl exists
            {
                for (int x = 0; x < Width; x++)
                    for (int z = 0; z < Depth; z++)
                        for (int y = 0; y < Height / 2; y++)
                            _SetBlock(new Vector3(x, y, z), new StoneBlock());
                for (int x = 0; x < Width; x++)
                    for (int z = 0; z < Depth; z++)
                        _SetBlock(new Vector3(x, Height / 2, z), new GrassBlock());
            }
            Spawn = SpawnLocation;
        }

        /// <summary>
        /// Gets or sets the level height.
        /// </summary>
        /// <value>The height.</value>
        /// <remarks></remarks>
        public short Height { get; set; }
        /// <summary>
        /// Gets or sets the level height.
        /// </summary>
        /// <value>The height.</value>
        /// <remarks></remarks>
        public short Width { get; set; }
        /// <summary>
        /// Gets or sets the level depth.
        /// </summary>
        /// <value>The depth.</value>
        /// <remarks></remarks>
        public short Depth { get; set; }
        /// <summary>
        /// Gets or sets the default world spawn.
        /// </summary>
        /// <value>The spawn.</value>
        /// <remarks></remarks>
        public Vector3 Spawn { get; set; }
        /// <summary>
        /// Gets the level data.
        /// </summary>
        /// <remarks></remarks>
        public byte[] Data { get; private set; }
        /// <summary>
        /// Gets or sets the world save directory.
        /// </summary>
        /// <value>The world directory.</value>
        /// <remarks></remarks>
        public string WorldFile { get; set; }
        /// <summary>
        /// Gets or sets the name of the level.
        /// </summary>
        /// <value>The name of the level.</value>
        /// <remarks></remarks>
        public string WorldName { get; set; }
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        /// <remarks></remarks>
        public Dictionary<string, string> Tags { get; set; }
        /// <summary>
        /// Occurs when a block is changed.
        /// </summary>
        /// <remarks></remarks>
        public event EventHandler<BlockSetEventArgs> OnBlockChanged;
        /// <summary>
        /// Gets a block from the world.
        /// </summary>
        /// <param name="position">The position of the requested block.</param>
        /// <returns>The block at the requested coordinates.</returns>
        /// <remarks>This will generate a new chunk for the requested block if one does not exist.</remarks>
        public virtual byte GetBlock(Vector3 position)
        {
            try
            {
                return Data[(int)position.Z + ((int)position.X * Height) + ((int)position.Y * Height * Depth)];
            }
            catch
            {
                return new AirBlock().BlockID; // wat do
            }
        }
        /// <summary>
        /// Sets a block in the world.
        /// </summary>
        /// <param name="position">The position of the updated block.</param>
        /// <param name="value">The block to set.</param>
        /// <remarks>This will generate a new chunk for the requested block if one does not exist.
        /// This method also fires OnBlockChanged, which will cause block updates when used with
        /// LibMinecraft.Server.MultiplayerServer.</remarks>
        public virtual void SetBlock(Vector3 position, Block value)
        {
            Data[(int)position.Z + ((int)position.X * Height) + ((int)position.Y * Height * Depth)] = value.BlockID;
            if (OnBlockChanged != null)
                OnBlockChanged(this, new BlockSetEventArgs(value, position));
        }

        /// <summary>
        /// Sets a block in the level without firing event handlers.
        /// </summary>
        /// <param name="position">The position of the updated block.</param>
        /// <param name="value">The block to set.</param>
        /// <remarks>This will generate a new chunk for the requested block if one does not exist.
        /// This method will not fire OnBlockChanged.</remarks>
        protected internal virtual void _SetBlock(Vector3 position, byte value)
        {
            Data[(int)position.Z + ((int)position.X * Height) + ((int)position.Y * Height * Depth)] = value;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <remarks>This will use whichever format (binary or NBT) that was
        /// last used to save this level.</remarks>
        public virtual void Save()
        {

        }

        /// <summary>
        /// Saves this world to a given directory
        /// in the LibMinecraftClassic-only binary format.
        /// </summary>
        /// <param name="TargetDirectory">The target directory.</param>
        /// <param name="FileName">Name of the file.</param>
        /// <remarks>The resulting files are not compatible with the official Minecraft software.</remarks>
        public virtual void SaveToBinary(string TargetDirectory, string FileName)
        {
            string Name = TargetDirectory + "\\" + FileName;
            var Binary = new BinaryWriter(File.Open(Name,FileMode.Create));

            try
            {
                Binary.Write(0x6c77f66e73); //Magic Number to make sure it is a compatible file.
                Binary.Write(Spawn.X + "!" + Spawn.Y + "!" + Spawn.Z);
                Binary.Write(Height + "@" + Width + "@" + Depth);
                Binary.Write(Tags.Count);
                foreach (var pair in Tags)
                {
                    Binary.Write(pair.Key);
                    Binary.Write(pair.Value);
                }
                Binary.Write(Compress(Data).Length);
                Binary.Write(Compress(Data));
                Binary.Write("EOF"); //EOF makes sure the entire file saved.
            }
            finally{
                Binary.Dispose();
            }
        }

        /// <summary>
        /// Loads the world in the LibMinecraftClassic-only
        /// binary format.
        /// </summary>
        /// <param name="TargetDirectory">The directory to load from</param>
        /// <param name="FileName">Name of the file.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual bool LoadFromBinary(string TargetDirectory, string FileName)
        {
            string Name = TargetDirectory + "\\" + FileName;
            try
            {
                var Binary = new BinaryReader(File.Open(Name, FileMode.Open));

                using (Binary)
                {
                    long v = Binary.ReadInt64();
                    if (v != 465869106803) //The magic number
                    {
                        throw new InvalidLevelException();
                    }
                    string s = Binary.ReadString();
                    this.Spawn = new Vector3(Convert.ToDouble(s.Split('!')[0]),
                        Convert.ToDouble(s.Split('!')[1]), Convert.ToDouble(s.Split('!')[2]));
                    s = Binary.ReadString();
                    this.Height = (short)Convert.ToInt32(s.Split('@')[0]);
                    this.Width = (short)Convert.ToInt32(s.Split('@')[1]);
                    this.Depth = (short)Convert.ToInt32(s.Split('@')[2]);

                    int count = Binary.ReadInt32();
                    // Read in all pairs.
                    for (int i = 0; i < count; i++)
                    {
                        string key = Binary.ReadString();
                        string value = Binary.ReadString();
                        Tags[key] = value;
                    }

                    int ByteLength = Binary.ReadInt32();
                    byte[] b = Decompress(Binary.ReadBytes(ByteLength));
                    this.Data = new byte[b.Length];
                    this.Data = b;

                    try
                    {
                        string EOF = Binary.ReadString();
                        if (EOF != "EOF")
                        {
                            throw new CorruptLevelException();
                        }
                    }
                    catch { throw new CorruptLevelException(); }
                }
                Binary.Dispose();
                return true;
            }
            catch { throw new FileNotFoundException(); }
        }

        #region Helper Methods
        /// <summary>
        /// Compresses the specified byte array.
        /// </summary>
        /// <param name="input">The byte array.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private byte[] Compress(byte[] input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream deflateStream = new GZipStream(ms, CompressionMode.Compress))
                {
                    deflateStream.Write(input, 0, input.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decompresses the specified byte array.
        /// </summary>
        /// <param name="gzip">The byte array.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// Finds the specified world.
        /// </summary>
        /// <param name="WorldName">Name of the world.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static World Find(string WorldName)
        {
            try
            {
                return Server.ClassicServer.Worlds.Find(delegate(World e)
                {
                    if (e.WorldName == WorldName)
                        return true;
                    else
                        return false;
                });
            }
            catch { throw new System.Collections.Generic.KeyNotFoundException(); }
        }

        /// <summary>
        /// Adds the world.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <remarks></remarks>
        public static void AddWorld(World world)
        {
            Server.ClassicServer.Worlds.Add(world);
        }
        #endregion
    }

    /// <summary>
    /// Represents a changed block in a level
    /// </summary>
    /// <remarks></remarks>
    public class BlockSetEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the block.
        /// </summary>
        /// <value>The block.</value>
        /// <remarks></remarks>
        public Block Block { get; set; }
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        /// <remarks></remarks>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockSetEventArgs"/> class.
        /// </summary>
        /// <param name="Block">The block.</param>
        /// <param name="Position">The position.</param>
        /// <remarks></remarks>
        public BlockSetEventArgs(Block Block, Vector3 Position)
        {
            this.Block = Block;
            this.Position = Position;
        }
    }

    /// <summary>
    /// Custom Exception Base
    /// </summary>
    /// <remarks></remarks>
    public class _ErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="_ErrorException"/> class.
        /// </summary>
        /// <remarks></remarks>
        public _ErrorException()
            : base() { }
    }

    /// <summary>
    /// Throws exception when level is invalid.
    /// </summary>
    /// <remarks></remarks>
    public class InvalidLevelException : _ErrorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLevelException"/> class.
        /// </summary>
        /// <remarks></remarks>
        public InvalidLevelException()
            : base() { }
    }

    /// <summary>
    /// Throws exception when level is corrupt.
    /// </summary>
    /// <remarks></remarks>
    public class CorruptLevelException : _ErrorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorruptLevelException"/> class.
        /// </summary>
        /// <remarks></remarks>
        public CorruptLevelException()
            : base() { }
    }
}
