using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model
{
    public class Level
    {
        public World Overworld { get; set; }
        public World Nether { get; set; }
        public World TheEnd { get; set; }

        public Difficulty Difficulty { get; set; }
        public bool MapFeatures { get; set; }
        public bool WeatherEnabled { get; set; }

        public GameMode GameMode { get; set; }
        public int GeneratorVersion { get; set; }

        public Vector3 Spawn { get; set; }
        public int Version { get; set; }
        public DateTime LastPlayed { get; set; }

        public long Seed { get; set; }
        public long Time { get; set; }

        public IWorldGenerator WorldGenerator { get; set; }
        public string Name { get; set; }

        public List<PlayerData> Players { get; set; }

        public event EventHandler<BlockChangeEventArgs> OnBlockChange;
        
        /// <summary>
        /// Fill this object with any data you like.  If it is
        /// serializable, it will be saved with the level.
        /// </summary>
        public Dictionary<string, object> Tags { get; set; }

        public Level(IWorldGenerator WorldGenerator)
        {
            this.WorldGenerator = WorldGenerator;

            Overworld = new World(this, Dimension.Overworld);
            Nether = new World(this, Dimension.Nether);
            TheEnd = new World(this, Dimension.TheEnd);

            Overworld.OnBlockChange += new EventHandler<BlockChangeEventArgs>(World_OnBlockChange);

            Spawn = new Vector3(0, 17, 0);

            GameMode = GameMode.Creative;
        }

        void World_OnBlockChange(object sender, BlockChangeEventArgs e)
        {
            if (OnBlockChange != null)
                OnBlockChange(sender, e);
        }

        public void SetBlock(Vector3 Location, Block Value)
        {
            Overworld.SetBlock(Location, Value);
        }

        public void SetBlock(Vector3 Location, Block Value, Dimension Dimension)
        {
            switch (Dimension)
            {
                case Dimension.Nether:
                    Nether.SetBlock(Location, Value);
                    break;
                case Dimension.TheEnd:
                    TheEnd.SetBlock(Location, Value);
                    break;
                default:
                    Overworld.SetBlock(Location, Value);
                    break;
            }
        }

        public Block GetBlock(Vector3 Location)
        {
            return Overworld.GetBlock(Location);
        }

        public Block GetBlock(Vector3 Location, Dimension Dimension)
        {
            switch (Dimension)
            {
                case Dimension.Overworld:
                    return Overworld.GetBlock(Location);
                case Dimension.Nether:
                    return Nether.GetBlock(Location);
                case Dimension.TheEnd:
                    return TheEnd.GetBlock(Location);
                default:
                    return Overworld.GetBlock(Location);
            }
        }

        public World GetWorld(Dimension Dimension)
        {
            switch (Dimension)
            {
                case Dimension.Overworld:
                    return Overworld;
                case Dimension.Nether:
                    return Nether;
                default:
                    return TheEnd;
            }
        }
    }
}
