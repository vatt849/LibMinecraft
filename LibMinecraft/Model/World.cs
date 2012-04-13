﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using System.ComponentModel;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using LibNbt;
using LibNbt.Tags;
using LibMinecraft.Model.Packets;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a Minecraft world, containing
    /// a number of chunks.
    /// </summary>
    /// <remarks></remarks>
    public class World : INotifyPropertyChanged
    {
        public const int Height = 256;

        public Level Level { get; set; }

        public List<Region> Regions { get; set; }

        /// <summary>
        /// Gets or sets the world save directory.
        /// </summary>
        /// <value>The world directory.</value>
        /// <remarks></remarks>
        public string WorldDirectory { get; set; }

        public Dimension Dimension { get; set; }

        internal List<Vector3> RegionsToSave;

        public List<Entity> Entities { get; set; }

        public event EventHandler<BlockChangeEventArgs> OnBlockChange;
        public event EventHandler<EntityEventArgs> OnEntityAdded;
        public event EventHandler<EntityEventArgs> OnEntityRemoved;

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        /// <remarks></remarks>
        public World(Level Level)
        {
            this.Level = Level;
            Dimension = Model.Dimension.Overworld;
            WorldDirectory = null;
            Regions = new List<Region>();
            RegionsToSave = new List<Vector3>();
            EnableUpdates = true;
            Entities = new List<Entity>();
        }

        public World(Level Level, Dimension Dimension) : this(Level)
        {
            this.Dimension = Dimension;
            switch (Dimension)
            {
                case Dimension.Overworld:
                    this.WorldDirectory = "region";
                    break;
                case Dimension.Nether:
                    this.WorldDirectory = "DIM-1";
                    break;
                case Dimension.TheEnd:
                    this.WorldDirectory = "DIM1";
                    break;
            }
        }

        /// <summary>
        /// (Re)generates a given column.  This does not update
        /// any connected clients on a server.
        /// </summary>
        /// <param name="Location"></param>
        public void GenerateColumn(Vector3 Location)
        {
            Region r = GetRegion(Location);
            if (!RegionsToSave.Contains(r.Location))
                RegionsToSave.Add(r.Location);
            Location -= r.Location;
            Vector3 coords = Location.Floor().Clone();
            coords.X = ((int)Location.X) % Region.Width;
            coords.Y = 0;
            coords.Z = ((int)Location.Z) % Region.Depth;
            if (Location.X < 0)
                coords.X = 32 + coords.X;
            if (Location.Z < 0)
                coords.Z = 32 + coords.Z;
            r.MapColumns.Add(Level.WorldGenerator.GenerateColumn(coords, r, Dimension, Level.Seed));
            lock (r.ColumnsToSave)
            {
                if (!r.ColumnsToSave.Contains(r.MapColumns.Last().Location))
                    r.ColumnsToSave.Add(r.MapColumns.Last().Location);
            }
        }

        public void AddEntity(Entity Entity)
        {
            this.Entities.Add(Entity);
            OnEntityAdded(this, new EntityEventArgs(Entity));
        }

        public void RemoveEntity(int ID)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i].ID == ID)
                {
                    OnEntityRemoved(this, new EntityEventArgs(Entities[i]));
                    Entities.Remove(Entities[i]);
                    return;
                }
            }
            throw new KeyNotFoundException("No entity with the given ID was found in this world.");
        }

        /// <summary>
        /// Gets a block from the world.
        /// </summary>
        /// <param name="position">The position of the requested block.</param>
        /// <returns>The block at the requested coordinates.</returns>
        /// <remarks>This will generate a new chunk for the requested block if one does not exist.</remarks>
        public virtual Block GetBlock(Vector3 position)
        {
            try
            {
                Region r = GetRegion(position);
                position -= r.Location;
                return r.GetBlock(position);
            }
            catch
            {
                return null; // or AirBlock?
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
            Region r = GetRegion(position);
            position -= r.Location;
            if (!RegionsToSave.Contains(r.Location))
                RegionsToSave.Add(r.Location);
            r.SetBlock(position, value);

            ScheduledUpdateManager.RemoveBlockUpdates(position);

            TriggerUpdates(position + r.Location);

            if (OnBlockChange != null)
                OnBlockChange(this, new BlockChangeEventArgs(value, position));
        }

        /// <summary>
        /// Sets a block in the world without firing event handlers.
        /// </summary>
        /// <param name="position">The position of the updated block.</param>
        /// <param name="value">The block to set.</param>
        /// <remarks>This will generate a new chunk for the requested block if one does not exist.
        /// This method will not fire OnBlockChanged or trigger block updates.</remarks>
        protected internal virtual void _SetBlock(Vector3 position, Block value)
        {
            Region r = GetRegion(position);
            position -= r.Location;
            if (!RegionsToSave.Contains(r.Location))
                RegionsToSave.Add(r.Location);
            r.SetBlock(position, value);
        }

        public bool EnableUpdates { get; set; }

        /// <summary>
        /// Triggers block updates near a given block.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public void TriggerUpdates(Vector3 location)
        {
            if (!EnableUpdates)
                return;

            Block b = this.GetBlock(location);
            b.BlockUpdate(this, location);

            b = this.GetBlock(location + Vector3.Up);
            b.BlockUpdate(this, location + Vector3.Up);

            b = this.GetBlock(location + Vector3.Down);
            b.BlockUpdate(this, location + Vector3.Down);

            b = this.GetBlock(location + Vector3.Right);
            b.BlockUpdate(this, location + Vector3.Right);

            b = this.GetBlock(location + Vector3.Left);
            b.BlockUpdate(this, location + Vector3.Left);

            b = this.GetBlock(location + Vector3.Forward);
            b.BlockUpdate(this, location + Vector3.Forward);

            b = this.GetBlock(location + Vector3.Backward);
            b.BlockUpdate(this, location + Vector3.Backward);
        }

        public MapColumn GetColumn(Vector3 position)
        {
            Region r = GetRegion(position);
            position -= r.Location;
            return r.GetColumn(position);
        }

        public Region GetRegion(Vector3 position)
        {
            Vector3 offset = position.Floor().Clone();
            offset.X = ((int)position.X) / (Region.Width * Chunk.Width);
            offset.Y = 0;
            offset.Z = ((int)position.Z) / (Region.Depth * Chunk.Depth);
            if (position.X < 0)
                offset.X--;
            if (position.Z < 0)
                offset.Z--;
            foreach (Region r in Regions)
            {
                if (r.Location.Equals(offset))
                    return r;
            }
            Region region = new Region(this, offset);
            Regions.Add(region);
            if (!RegionsToSave.Contains(region.Location))
                RegionsToSave.Add(region.Location);
            return region;
        }

        public Chunk GetChunk(Vector3 Location)
        {
            Region r = GetRegion(Location);
            Location -= r.Location;
            MapColumn mc = r.GetColumn(Location);
            Location -= mc.Location;
            return mc.GetChunk(Location);
        }

        public void SetChunk(Vector3 Location, Chunk c)
        {
            Region r = GetRegion(Location);
            Location -= r.Location;
            if (!RegionsToSave.Contains(r.Location))
                RegionsToSave.Add(r.Location);
            r.GetColumn(Location).SetChunk(Location, c);
        }

        public void UpdateEntities()
        {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].Tick(this);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public virtual void Save()
        {
            if (string.IsNullOrEmpty(this.WorldDirectory))
                return;
            if (!Directory.Exists(this.WorldDirectory))
                Directory.CreateDirectory(this.WorldDirectory);
            foreach (Vector3 regionLocation in RegionsToSave)
            {
                Region region = GetRegion(regionLocation);
                string regionFile = Path.Combine(WorldDirectory,
                    string.Format("r.{0}.{1}.mca", region.Location.X, region.Location.Z));
                if (!File.Exists(regionFile))
                    CreateEmptyRegion(regionFile);

                // Create the region's NBT blobs
                lock (region.ColumnsToSave)
                {
                    foreach (Vector3 mapColumn in region.ColumnsToSave)
                    {
                        NbtFile regionNbt = new NbtFile();
                        regionNbt.RootTag = new NbtCompound();

                        MapColumn mc = region.GetColumn(mapColumn);
                        NbtCompound level = new NbtCompound("Level");
                        level.Tags.Add(new NbtByte("TerrainPopulated", 1));
                        level.Tags.Add(new NbtInt("xPos", (int)region.Location.X));
                        level.Tags.Add(new NbtInt("zPos", (int)region.Location.Z));
                        level.Tags.Add(new NbtLong("LastUpdate", region.LastUpdated.Ticks));
                        level.Tags.Add(new NbtByteArray("Biomes", mc.Biomes));
                        NbtList entityList = new NbtList("Entities");
                        Cuboid columnCollisionBox = new Cuboid(mc.Location + region.Location, new Vector3(16, 256, 16));
                        foreach (Entity entity in Entities.Where(e => !(e is PlayerEntity)
                            && !(e is FallingEnderDragonEggEntity)
                            && !(e is FallingSandEntity)
                            && !(e is FallingGravelEntity) // TODO: Add a mob entity meta-class and only save it
                            && e.CollisionBox.Intersects(columnCollisionBox)))
                        {
                            entityList.Tags.Add(entity.GetEntityCompound());
                        }
                        level.Tags.Add(entityList);
                        NbtList sections = new NbtList("Sections");
                        foreach (Chunk chunk in mc.Chunks.Where(c => !c.IsAir))
                        {
                            NbtCompound chunkCompound = new NbtCompound();
                            chunkCompound.Tags.Add(new NbtByte("Y", (byte)(chunk.Location.Y / 16)));
                            chunkCompound.Tags.Add(new NbtByteArray("BlockLight", chunk.BlockLight));
                            chunkCompound.Tags.Add(new NbtByteArray("Blocks", chunk.Blocks));
                            chunkCompound.Tags.Add(new NbtByteArray("Data", chunk.Metadata));
                            chunkCompound.Tags.Add(new NbtByteArray("SkyLight", chunk.SkyLight));
                            sections.Tags.Add(chunkCompound);
                        }
                        level.Tags.Add(sections);
                        // TODO: Tile Entities
                        level.Tags.Add(new NbtByteArray("HeightMap", mc.HeightMap));
                        regionNbt.RootTag.Tags.Add(level);

                        MemoryStream compressionStream = new MemoryStream();
                        regionNbt.SaveFile(compressionStream, true);
                        byte[] compressedNbt = compressionStream.GetBuffer();
                        int actualLength = compressedNbt.Length;
                        if (compressedNbt.Length % 4096 != 0)
                            compressedNbt = compressedNbt.Concat(new byte[compressedNbt.Length % 4096]).ToArray();

                        using (Stream regionStream = File.Open(regionFile, FileMode.Open))
                        {
                            long tableOffset = (long)(mc.Location.X  * 32 + mc.Location.Z);

                            // Search for a new sector to hold the NBT
                            byte neededSectors = (byte)(compressedNbt.Length / 4096 + 1);
                            long newSectorOffset = regionStream.Length;

                            regionStream.Seek(0, SeekOrigin.Begin);
                            int previousLocation = sizeof(int) * 32 * 32 * 2;
                            byte previousLength = 0;

                            bool columnWritten = false;
                            while (regionStream.Position < sizeof(int) * 32 * 32) // Read through the header
                            {
                                int value = Packet.ReadInt(regionStream);
                                byte usedSectors = (byte)(value & 0xFF);
                                int sectorOffset = value >> 8;
                                if (usedSectors != 0 && sectorOffset - (previousLocation + (previousLength * 4096)) < neededSectors)
                                {
                                    // We passed an empty sector large enough to hold the chunk
                                    newSectorOffset = previousLocation + (previousLength * 4096);
                                    break;
                                }
                                previousLength = usedSectors;
                                previousLocation = sectorOffset;
                            }

                            // newSectorOffset should point to a good place to write the column to
                            regionStream.Seek(newSectorOffset, SeekOrigin.Begin);

                            byte[] sectionHeader = Packet.MakeInt(actualLength).Concat(new byte[] { 1 }).ToArray();
                            compressedNbt = sectionHeader.Concat(compressedNbt).ToArray();

                            regionStream.Write(compressedNbt, 0, compressedNbt.Length);
                            regionStream.Seek(tableOffset, SeekOrigin.Begin);
                            int tableValue = (int)(newSectorOffset << 8 | neededSectors);
                            Packet.WriteInt(regionStream, tableValue);
                            regionStream.Flush();
                            regionStream.Close();
                        }
                    }
                }

                region.ColumnsToSave.Clear();
            }
        }

        private void CreateEmptyRegion(string regionFile)
        {
            using (Stream regionStream = File.Open(regionFile, FileMode.Create))
            {
                // Create blank region header
                regionStream.Write(new byte[sizeof(int) * 32 * 32 * 2], 0, sizeof(int) * 32 * 32 * 2); // TODO: Timestamps
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks></remarks>
        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Represents a changed block in a world
    /// </summary>
    /// <remarks></remarks>
    public class BlockChangeEventArgs : EventArgs
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
        /// Initializes a new instance of the <see cref="BlockChangeEventArgs"/> class.
        /// </summary>
        /// <param name="Block">The block.</param>
        /// <param name="Position">The position.</param>
        /// <remarks></remarks>
        public BlockChangeEventArgs(Block Block, Vector3 Position)
        {
            this.Block = Block;
            this.Position = Position;
        }
    }

    /// <summary>
    /// Represents a new entity in a world.
    /// </summary>
    /// <remarks></remarks>
    public class EntityEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>The entity.</value>
        /// <remarks></remarks>
        public Entity Entity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEventArgs"/> class.
        /// </summary>
        /// <param name="Entity">The entity.</param>
        /// <remarks></remarks>
        public EntityEventArgs(Entity Entity)
        {
            this.Entity = Entity;
        }
    }

    /// <summary>
    /// Represents all gamemodes available in the official Minecraft software.
    /// </summary>
    /// <remarks></remarks>
    public enum GameMode
    {
        /// <summary>
        /// 
        /// </summary>
        Survival = 0,
        /// <summary>
        /// 
        /// </summary>
        Creative = 1,
        /// <summary>
        /// 
        /// </summary>
        Hardcore = 2,
    }

    /// <summary>
    /// Represents all default dimensions.
    /// </summary>
    /// <remarks></remarks>
    public enum Dimension
    {
        /// <summary>
        /// 
        /// </summary>
        Nether = -1,
        /// <summary>
        /// 
        /// </summary>
        Overworld = 0,
        /// <summary>
        /// 
        /// </summary>
        TheEnd = 1,
    }
}
