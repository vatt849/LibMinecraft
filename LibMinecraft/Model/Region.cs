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

        public bool IsGenerated { get; set; } // TODO: Use this value

        public DateTime LastUpdated { get; set; } // TODO: Use this value

        public Vector3 Location { get; set; }

        public List<MapColumn> MapColumns;

        public World World { get; set; }

        public List<Vector3> ColumnsToSave { get; set; }

        public Region(World World, Vector3 Location)
        {
            this.World = World;
            MapColumns = new List<MapColumn>();
            ColumnsToSave = new List<Vector3>();
            this.Location = Location;
        }

        public Block GetBlock(Vector3 Location)
        {
            Location.X = ((int)Location.X) % Region.Width / 16 * 16;
            Location.Y = 0;
            Location.Z = ((int)Location.Z) % Region.Depth / 16 * 16;
            MapColumn mc = GetColumn(Location);
            Location -= mc.Location;
            return mc.GetBlock(Location);
        }

        public void SetBlock(Vector3 Location, Block Value)
        {
            Vector3 columnLocation = Location.Clone();
            columnLocation.X = ((int)Location.X) % Region.Width / 16 * 16;
            columnLocation.Y = 0;
            columnLocation.Z = ((int)Location.Z) % Region.Depth / 16 * 16;
            MapColumn mc = GetColumn(columnLocation);
            Location -= mc.Location;
            lock (ColumnsToSave)
            {
                if (!ColumnsToSave.Contains(mc.Location))
                    ColumnsToSave.Add(mc.Location);
            }
            mc.SetBlock(Location, Value);
        }

        /// <summary>
        /// Retrieves the MapColumn at the specified
        /// relative coordinates.  Location.Y is discarded.
        /// </summary>
        /// <param name="Location"></param>
        public MapColumn GetColumn(Vector3 Location)
        {
            // Align the location to a column
            Vector3 coords = Location.Floor().Clone();
            coords.X = ((int)Location.X) % Region.Width;
            coords.Y = 0;
            coords.Z = ((int)Location.Z) % Region.Depth;
            if (Location.X < 0)
                coords.X = 32 + coords.X;
            if (Location.Z < 0)
                coords.Z = 32 + coords.Z;
            // Retrieve a column, or generate a new one
            foreach (MapColumn c in MapColumns)
            {
                if (c.Location.Equals(coords))
                    return c;
            }
            
            // Generate a column
            MapColumn mc = this.World.Level.WorldGenerator.GenerateColumn(coords, this,
                this.World.Dimension, this.World.Level.Seed);
            mc.Location = coords;
            MapColumns.Add(mc);
            lock (ColumnsToSave)
            {
                if (!ColumnsToSave.Contains(mc.Location))
                    ColumnsToSave.Add(mc.Location);
            }
            return mc;
        }

        public void GenerateColumn(Vector3 Location)
        {
            // Align the location to a column
            Vector3 coords = Location.Floor().Clone();
            coords.X = ((int)Location.X) % Region.Width;
            coords.Y = 0;
            coords.Z = ((int)Location.Z) % Region.Depth;
            if (Location.X < 0)
                coords.X = 32 + coords.X;
            if (Location.Z < 0)
                coords.Z = 32 + coords.Z;
            if ((coords.X == 0 || coords.Z == 0) && this.Location.X == -1 && this.Location.Z == -1)
                System.Diagnostics.Debugger.Break();

            if (this.MapColumns.Where(m => m.Location == coords).Count() != 0)
                this.MapColumns.Remove(this.MapColumns.Where(m => m.Location == coords).First());

            // Generate a column
            MapColumn mc = this.World.Level.WorldGenerator.GenerateColumn(coords, this,
                this.World.Dimension, this.World.Level.Seed);
            mc.Location = coords;
            MapColumns.Add(mc);
            lock (ColumnsToSave)
            {
                if (!ColumnsToSave.Contains(mc.Location))
                    ColumnsToSave.Add(mc.Location);
            }
        }
    }
}
