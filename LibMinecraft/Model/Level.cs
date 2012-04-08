using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using System.ComponentModel;
using System.IO;
using LibNbt;
using LibNbt.Tags;
using LibMinecraft.Server;

namespace LibMinecraft.Model
{
    public class Level : INotifyPropertyChanged
    {

        public World Overworld { get; set; }
        public World Nether { get; set; }
        public World TheEnd { get; set; }

        public Difficulty Difficulty { get; set; }
        public bool MapFeatures { get; set; }
        public WeatherManager WeatherManager { get; set; }

        public GameMode GameMode { get; set; }
        public int GeneratorVersion { get; set; }

        public Vector3 Spawn { get; set; }
        public int Version { get; set; }

        private DateTime _LastPlayed;
        public DateTime LastPlayed
        {
            get
            {
                return _LastPlayed;
            }
            set
            {
                if (!Equals(_LastPlayed, value) && PropertyChanged != null)
                {
                    _LastPlayed = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("LastPlayed"));
                }
                else
                    _LastPlayed = value;
            }
        }

        private long _Seed;
        public long Seed
        {
            get
            {
                return _Seed;
            }
            set
            {
                if (!Equals(_Seed, value) && PropertyChanged != null)
                {
                    _Seed = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Seed"));
                }
                else
                    _Seed = value;
            }
        }


        internal long _Time;
        public long Time
        {
            get
            {
                return _Time;
            }
            set
            {
                if (!Equals(_Time, value) && PropertyChanged != null)
                {
                    _Time = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Time"));
                }
                else
                    _Time = value;
            }
        }

        public IWorldGenerator WorldGenerator { get; set; }
        public string Name { get; set; }
        public string SaveDirectory { get; set; }

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

            WeatherManager = new WeatherManager();
        }

        public void Save(string SaveDirectory)
        {
            this.SaveDirectory = SaveDirectory;
            Overworld.WorldDirectory = Path.Combine(SaveDirectory, "region");
            if (!Directory.Exists(Path.Combine(SaveDirectory, "DIM1")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "DIM1"));
            Nether.WorldDirectory = Path.Combine(SaveDirectory, "DIM1", "region");
            if (!Directory.Exists(Path.Combine(SaveDirectory, "DIM-1")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "DIM-1"));
            Nether.WorldDirectory = Path.Combine(SaveDirectory, "DIM-1", "region");

            Save();
        }

        public void Save()
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
            if (!Directory.Exists(Path.Combine(SaveDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "data"));
            if (!Directory.Exists(Path.Combine(SaveDirectory, "players")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "players"));

            NbtFile file = new NbtFile("level.dat");
            file.RootTag = new NbtCompound("Data");
            file.RootTag.Set("hardcore", new NbtByte((byte)(this.GameMode == GameMode.Hardcore ? 1 : 0)));
            file.RootTag.Set("MapFeatures", new NbtByte((byte)(WorldGenerator.GenerateStructures ? 1 : 0)));
            file.RootTag.Set("raining", new NbtByte((byte)(WeatherManager.WeatherOccuring ? 1 : 0)));
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

        public IEnumerable<World> Worlds
        {
            get
            {
                return new World[] { Overworld, Nether, TheEnd };
            }
        }
    
        public event PropertyChangedEventHandler  PropertyChanged;
    }
}
