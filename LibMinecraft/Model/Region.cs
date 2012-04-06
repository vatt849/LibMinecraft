using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a 32x32 group of map columns
    /// </summary>
    public class Region
    {
        /// <summary>
        /// The default Width and Depth for a region.
        /// Width = 32;
        /// Depth = 32;
        /// </summary>
        public const int Width = 32, Depth = 32;

        public Vector3 Location { get; set; }

        public List<MapColumn> MapColumns;

        public World World { get; set; }

        public Region(World World, Vector3 Location)
        {
            this.World = World;
            MapColumns = new List<MapColumn>();
            this.Location = Location;
        }

        public Block GetBlock(Vector3 Location)
        {
            MapColumn mc = GetColumn(Location);
            Location -= mc.Location;
            return mc.GetBlock(Location);
        }

        public void SetBlock(Vector3 Location, Block Value)
        {
            MapColumn mc = GetColumn(Location);
            Location -= mc.Location;
            mc.SetBlock(Location, Value);
        }

        /// <summary>
        /// Retrieves the MapColumn at the specified
        /// relative coordinates.  Location.Y is discarded.
        /// </summary>
        /// <param name="Location"></param>
        public MapColumn GetColumn(Vector3 Location)
        {
            // Align the location to a chunk
            Vector3 coords = Location.Clone();
            coords.X = ((int)coords.X & ~0xf);
            coords.Y = 0;
            coords.Z = ((int)coords.Z & ~0xf);
            // Retrieve a column, or generate a new one
            foreach (MapColumn c in MapColumns)
            {
                if (c.Location.Equals(coords))
                    return c;
            }
            
            // Generate a column
            MapColumn mc = new MapColumn(this, coords);
            mc = this.World.Level.WorldGenerator.GenerateColumn(mc.Location, this,
                this.World.Dimension, this.World.Level.Seed);
            MapColumns.Add(mc);
            return mc;
        }
    }
}
