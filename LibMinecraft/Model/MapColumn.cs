using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents 16 vertically stacked chunks
    /// </summary>
    public class MapColumn
    {
        public Chunk[] Chunks { get; set; }
        public Vector3 Location { get; set; }
        public bool IsGenerated { get; set; }
        public long LastUpdate { get; set; }
        public byte[] Biomes { get; set; }
        public int[] HeightMap { get; set; }
        public Region Region { get; set; }
        public List<Entity> Entities { get; set; }

        public MapColumn(Region Region, Vector3 Location)
        {
            this.Region = Region;
            this.Location = Location;
            Chunks = new Chunk[16];
            for (int i = 0; i < Chunks.Length; i++)
                Chunks[i] = new Chunk(this, this.Location + new Vector3(0, i * 16, 0));
            Biomes = new byte[256];
            HeightMap = new int[256];
            IsGenerated = false;
            Entities = new List<Entity>();
        }

        public Block GetBlock(Vector3 Location)
        {
            int index = (int)Location.Y / 16;
            return Chunks[index].GetBlock(new Vector3(Location.X, ((int)Location.Y) % 16, Location.Z));
        }

        public void SetBlock(Vector3 Location, Block Value)
        {
            int index = (int)Location.Y / 16;
            int heightIndex = (int)(Location.X * Chunk.Width + Location.Z);
            if (Value is AirBlock && HeightMap[heightIndex] == (byte)Location.Y)
            {
                int newHeight = HeightMap[heightIndex];
                for (int y = HeightMap[heightIndex]; y > 0; y++)
                    if (GetBlock(new Vector3(Location.X, y, Location.Z)) != 0)
                    {
                        newHeight = y;
                        break;
                    }
            }
            else
            {
                if (Location.Y > HeightMap[heightIndex])
                    HeightMap[heightIndex] = (byte)Location.Y;
            }
            Chunks[index].SetBlock(new Vector3(Location.X, ((int)Location.Y) % 16, Location.Z), Value);
        }

        public Chunk GetChunk(Vector3 Location)
        {
            int index = (int)Location.Y / 16;
            return Chunks[index];
        }

        public void SetChunk(Vector3 Location, Chunk Chunk)
        {
            int index = (int)Location.Y / 16;
            Chunks[index].Location = Location;
            Chunks[index] = Chunk;
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }
}
