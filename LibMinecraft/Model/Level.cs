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
using LibMinecraft.Model.Entities;

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
            Difficulty = Model.Difficulty.Normal;

            WeatherManager = new WeatherManager();

            Name = "world";
        }

        public void SavePlayer(RemoteClient rc)
        {
            NbtFile player = new NbtFile(rc.PlayerEntity.Name + ".dat");
            player.RootTag = new NbtCompound();
            player.RootTag.Tags.Add(new NbtByte("OnGround", (byte)(rc.PlayerEntity.OnGround ? 1 : 0)));
            player.RootTag.Tags.Add(new NbtByte("Sleeping", (byte)(rc.PlayerEntity.Sleeping ? 1 : 0)));
            player.RootTag.Tags.Add(new NbtShort("Air", 0)); // TODO
            player.RootTag.Tags.Add(new NbtShort("AttackTime", 0)); // TODO
            player.RootTag.Tags.Add(new NbtShort("Fire", rc.PlayerEntity.TimeOnFire));
            player.RootTag.Tags.Add(new NbtShort("Health", rc.PlayerEntity.Health));
            player.RootTag.Tags.Add(new NbtShort("HurtTime", 0)); // TODO
            player.RootTag.Tags.Add(new NbtShort("SleepTimer", rc.PlayerEntity.SleepTimer));
            player.RootTag.Tags.Add(new NbtInt("Dimension", (int)rc.PlayerEntity.Dimension));
            player.RootTag.Tags.Add(new NbtInt("foodLevel", (int)rc.PlayerEntity.Food));
            player.RootTag.Tags.Add(new NbtInt("foodTickTimer", (int)rc.PlayerEntity.FoodTimer));
            player.RootTag.Tags.Add(new NbtInt("playerGameType", (int)rc.PlayerEntity.GameMode));
            player.RootTag.Tags.Add(new NbtInt("XpLevel", rc.PlayerEntity.XpLevel));
            player.RootTag.Tags.Add(new NbtInt("XpTotal", rc.PlayerEntity.XpTotal));

            NbtList inventory = new NbtList("Inventory");
            for (int i = 0; i < rc.PlayerEntity.Inventory.Length; i++)
            {
                NbtCompound item = new NbtCompound();
                item.Tags.Add(new NbtByte("Count", rc.PlayerEntity.Inventory[i].Count));
                item.Tags.Add(new NbtByte("Slot", (byte)i));
                item.Tags.Add(new NbtShort("Damage", rc.PlayerEntity.Inventory[i].Metadata));
                item.Tags.Add(new NbtShort("id", rc.PlayerEntity.Inventory[i].ID));
                inventory.Tags.Add(item);
            }
            player.RootTag.Tags.Add(inventory);

            NbtList motion = new NbtList("Motion");
            motion.Tags.Add(new NbtDouble(rc.PlayerEntity.Velocity.X));
            motion.Tags.Add(new NbtDouble(rc.PlayerEntity.Velocity.Y));
            motion.Tags.Add(new NbtDouble(rc.PlayerEntity.Velocity.Z));
            player.RootTag.Tags.Add(motion);

            NbtList position = new NbtList("Position");
            position.Tags.Add(new NbtDouble(rc.PlayerEntity.Location.X));
            position.Tags.Add(new NbtDouble(rc.PlayerEntity.Location.Y));
            position.Tags.Add(new NbtDouble(rc.PlayerEntity.Location.Z));
            player.RootTag.Tags.Add(position);

            NbtList rotation = new NbtList("Rotation");
            rotation.Tags.Add(new NbtFloat((float)rc.PlayerEntity.Rotation.X));
            rotation.Tags.Add(new NbtFloat((float)rc.PlayerEntity.Rotation.Y));
            player.RootTag.Tags.Add(rotation);

            player.SaveFile(Path.Combine(SaveDirectory, "players", rc.PlayerEntity.Name + ".dat"));
        }

        public PlayerEntity LoadPlayer(string Name)
        {
            PlayerEntity entity = new PlayerEntity();
            entity.Location = Spawn;
            entity.Name = Name;
            entity.ID = MultiplayerServer.NextEntityID++;
            entity.Health = 20;
            entity.Food = 20;
            entity.FoodSaturation = 1;
            entity.LevelIndex = 0; // TODO: Remember which level they are saved in
            entity.LoadedColumns = new List<Vector3>();

            if (string.IsNullOrEmpty(SaveDirectory) || !File.Exists(Path.Combine(SaveDirectory, "players", entity.Name + ".dat")))
                return entity;

            NbtFile file = new NbtFile();
            file.LoadFile(Path.Combine(SaveDirectory, "players", entity.Name + ".dat"));

            if (file.Query<NbtByte>("//OnGround") != null)
                entity.OnGround = file.Query<NbtByte>("//OnGround").Value == 1;
            if (file.Query<NbtByte>("//Sleeping") != null)
                entity.Sleeping = file.Query<NbtByte>("//Sleeping").Value == 1;
            // TODO: Commented values
            //entity.Air
            //entity.AttackTime
            //entity.DeathTime
            if (file.Query<NbtShort>("//Fire") != null)
                entity.TimeOnFire = file.Query<NbtShort>("//Fire").Value;
            if (file.Query<NbtShort>("//Health") != null)
                entity.Health = file.Query<NbtShort>("//Health").Value;
            //entity.HurtTime
            if (file.Query<NbtShort>("//SleepTimer") != null)
                entity.SleepTimer = file.Query<NbtShort>("//SleepTimer").Value;
            if (file.Query<NbtInt>("//Dimension") != null)
                entity.Dimension = (Dimension)file.Query<NbtInt>("//Dimension").Value;
            if (file.Query<NbtInt>("//foodLevel") != null)
                entity.Food = (short)file.Query<NbtInt>("//foodLevel").Value;
            if (file.Query<NbtInt>("//foodTickTimer") != null)
                entity.FoodTimer = file.Query<NbtInt>("//foodTickTimer").Value;
            if (file.Query<NbtInt>("//playerGameType") != null)
                entity.GameMode = (GameMode)file.Query<NbtInt>("//playerGameType").Value;
            if (file.Query<NbtInt>("//XpLevel") != null)
                entity.XpLevel = file.Query<NbtInt>("//XpLevel").Value;
            if (file.Query<NbtInt>("//XpTotal") != null)
                entity.XpTotal = file.Query<NbtInt>("//XpTotal").Value;
            //entity.FallDistance
            if (file.Query<NbtFloat>("//foodExhastionLevel") != null)
                entity.FoodExaustionLevel = file.Query<NbtFloat>("//foodExhastionLevel").Value;
            if (file.Query<NbtFloat>("//foodSaturationLevel") != null)
                entity.FoodSaturation = file.Query<NbtFloat>("//foodSaturationLevel").Value;

            NbtList inventory = file.Query<NbtList>("//Inventory");
            foreach (NbtCompound slot in inventory.Tags.Where(t => t is NbtCompound))
            {
                try
                {
                    Slot s = new Slot();
                    s.ID = slot.Query<NbtShort>("//id").Value;
                    s.Count = slot.Query<NbtByte>("//Count").Value;
                    s.Metadata = slot.Query<NbtShort>("//Damage").Value;
                    entity.Inventory[slot.Query<NbtByte>("//Slot").Value] = s;
                }
                catch { }
            }

            if (file.Query<NbtDouble>("//Motion/0") != null)
                entity.Velocity.X = file.Query<NbtDouble>("//Motion/0").Value;
            if (file.Query<NbtDouble>("//Motion/1") != null)
                entity.Velocity.Y = file.Query<NbtDouble>("//Motion/1").Value;
            if (file.Query<NbtDouble>("//Motion/2") != null)
                entity.Velocity.Z = file.Query<NbtDouble>("//Motion/2").Value;

            if (file.Query<NbtDouble>("//Position/0") != null)
                entity.Location.X = file.Query<NbtDouble>("//Position/0").Value;
            if (file.Query<NbtDouble>("//Position/1") != null)
                entity.Location.Y = file.Query<NbtDouble>("//Position/1").Value;
            if (file.Query<NbtDouble>("//Position/2") != null)
                entity.Location.Z = file.Query<NbtDouble>("//Position/2").Value;

            if (file.Query<NbtFloat>("//Rotation/0") != null)
                entity.Rotation.X = file.Query<NbtFloat>("//Rotation/0").Value;
            if (file.Query<NbtFloat>("//Rotation/1") != null)
                entity.Rotation.Y = file.Query<NbtFloat>("//Rotation/1").Value;

            return entity;
        }

        public void Save(string SaveDirectory, MultiplayerServer server)
        {
            this.SaveDirectory = SaveDirectory;
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
            Overworld.WorldDirectory = Path.Combine(SaveDirectory, "region");
            if (!Directory.Exists(Path.Combine(SaveDirectory, "DIM1")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "DIM1"));
            Nether.WorldDirectory = Path.Combine(SaveDirectory, "DIM1", "region");
            if (!Directory.Exists(Path.Combine(SaveDirectory, "DIM-1")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "DIM-1"));
            TheEnd.WorldDirectory = Path.Combine(SaveDirectory, "DIM-1", "region");
            if (!Directory.Exists(Path.Combine(SaveDirectory, "players")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "players"));

            Save(server);
        }

        public void Save(MultiplayerServer server) // TODO: Singleplayer world save methods (and generic non-server/client dependent save methods)
        {
            if (string.IsNullOrEmpty(SaveDirectory))
                return;
            if (!Directory.Exists(Path.Combine(SaveDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "data"));
            if (!Directory.Exists(Path.Combine(SaveDirectory, "players")))
                Directory.CreateDirectory(Path.Combine(SaveDirectory, "players"));

            NbtFile file = new NbtFile("level.dat");
            file.RootTag = new NbtCompound("Data");
            file.RootTag.Tags.Add(new NbtByte("hardcore", (byte)(this.GameMode == GameMode.Hardcore ? 1 : 0)));
            file.RootTag.Tags.Add(new NbtByte("MapFeatures", (byte)(WorldGenerator.GenerateStructures ? 1 : 0)));
            file.RootTag.Tags.Add(new NbtByte("raining", (byte)(WeatherManager.WeatherOccuring ? 1 : 0)));
            file.RootTag.Tags.Add(new NbtByte("thundering", (byte)(WeatherManager.EnableThunder ? 1 : 0)));
            file.RootTag.Tags.Add(new NbtInt("GameType", (int)GameMode));
            file.RootTag.Tags.Add(new NbtInt("generatorVersion", 0));
            file.RootTag.Tags.Add(new NbtInt("rainTime", WeatherManager.TicksUntilUpdate));
            file.RootTag.Tags.Add(new NbtInt("SpawnX", (int)Spawn.X));
            file.RootTag.Tags.Add(new NbtInt("SpawnY", (int)Spawn.Y));
            file.RootTag.Tags.Add(new NbtInt("SpawnZ", (int)Spawn.Z));
            file.RootTag.Tags.Add(new NbtInt("thunderTime", WeatherManager.TicksUntilThunder));
            file.RootTag.Tags.Add(new NbtInt("version", 19113));
            file.RootTag.Tags.Add(new NbtLong("LastPlayed", DateTime.Now.ToFileTimeUtc()));
            file.RootTag.Tags.Add(new NbtLong("RandomSeed", Seed));
            file.RootTag.Tags.Add(new NbtLong("SizeOnDisk", 0));
            file.RootTag.Tags.Add(new NbtLong("Time", Time));
            file.RootTag.Tags.Add(new NbtString("generatorName", WorldGenerator.Name));
            file.RootTag.Tags.Add(new NbtString("levelName", Name));
            file.SaveFile(Path.Combine(SaveDirectory, "level.dat"));

            foreach (RemoteClient rc in server.GetClientsInLevel(this))
            {
                NbtFile player = new NbtFile(rc.PlayerEntity.Name + ".dat");
                player.RootTag = new NbtCompound();
                player.RootTag.Tags.Add(new NbtByte("OnGround", (byte)(rc.PlayerEntity.OnGround ? 1 : 0)));
                player.RootTag.Tags.Add(new NbtByte("Sleeping", (byte)(rc.PlayerEntity.Sleeping ? 1 : 0)));
                player.RootTag.Tags.Add(new NbtShort("Air", 0)); // TODO
                player.RootTag.Tags.Add(new NbtShort("AttackTime", 0)); // TODO
                player.RootTag.Tags.Add(new NbtShort("Fire", rc.PlayerEntity.TimeOnFire));
                player.RootTag.Tags.Add(new NbtShort("Health", rc.PlayerEntity.Health));
                player.RootTag.Tags.Add(new NbtShort("HurtTime", 0)); // TODO
                player.RootTag.Tags.Add(new NbtShort("SleepTimer", rc.PlayerEntity.SleepTimer));
                player.RootTag.Tags.Add(new NbtInt("Dimension", (int)rc.PlayerEntity.Dimension));
                player.RootTag.Tags.Add(new NbtInt("foodLevel", (int)rc.PlayerEntity.Food));
                player.RootTag.Tags.Add(new NbtInt("foodTickTimer", (int)rc.PlayerEntity.FoodTimer));
                player.RootTag.Tags.Add(new NbtInt("playerGameType", (int)rc.PlayerEntity.GameMode));
                player.RootTag.Tags.Add(new NbtInt("XpLevel", rc.PlayerEntity.XpLevel));
                player.RootTag.Tags.Add(new NbtInt("XpTotal", rc.PlayerEntity.XpTotal));

                NbtList inventory = new NbtList("Inventory");
                for (int i = 0; i < rc.PlayerEntity.Inventory.Length; i++)
                {
                    NbtCompound item = new NbtCompound();
                    item.Tags.Add(new NbtByte("Count", rc.PlayerEntity.Inventory[i].Count));
                    item.Tags.Add(new NbtByte("Slot", (byte)i));
                    item.Tags.Add(new NbtShort("Damage", rc.PlayerEntity.Inventory[i].Metadata));
                    item.Tags.Add(new NbtShort("id", rc.PlayerEntity.Inventory[i].ID));
                    inventory.Tags.Add(item);
                }
                player.RootTag.Tags.Add(inventory);

                NbtList motion = new NbtList("Motion");
                motion.Tags.Add(new NbtDouble(rc.PlayerEntity.Velocity.X));
                motion.Tags.Add(new NbtDouble(rc.PlayerEntity.Velocity.Y));
                motion.Tags.Add(new NbtDouble(rc.PlayerEntity.Velocity.Z));
                player.RootTag.Tags.Add(motion);

                NbtList position = new NbtList("Position");
                position.Tags.Add(new NbtDouble(rc.PlayerEntity.Location.X));
                position.Tags.Add(new NbtDouble(rc.PlayerEntity.Location.Y));
                position.Tags.Add(new NbtDouble(rc.PlayerEntity.Location.Z));
                player.RootTag.Tags.Add(position);

                NbtList rotation = new NbtList("Rotation");
                rotation.Tags.Add(new NbtFloat((float)rc.PlayerEntity.Rotation.X));
                rotation.Tags.Add(new NbtFloat((float)rc.PlayerEntity.Rotation.Y));
                player.RootTag.Tags.Add(rotation);

                player.SaveFile(Path.Combine(SaveDirectory, "players", rc.PlayerEntity.Name + ".dat"));
            }

            Overworld.Save();
            Nether.Save();
            TheEnd.Save();
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
