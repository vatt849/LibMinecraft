using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Entities
{
    public class PlayerEntity : Entity, INotifyPropertyChanged
    {
        public const short CraftingOutput = 0;
        public const short CraftingOffset = 1;
        public const short ArmorOffset = 5;
        public const short InventoryOffset = 9;
        public const short HotbarOffset = 36;
        public const short InventoryLength = 45;

        public PlayerEntity() : base()
        {
            Location = new Vector3();
            OldLocation = new Vector3();
            Velocity = new Vector3();
            Rotation = new Vector3();
            MapLoadRadius = 3;
            MapLoadRadiusDueTime = 50;
            SelectedSlot = HotbarOffset;
            Inventory = new Slot[45];
            for (int i = 0; i < Inventory.Length; i++)
                Inventory[i] = new Slot(-1, 1);
        }

        public bool Sleeping { get; set; }
        public short SleepTimer { get; set; }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (!Equals(_Name, value) && PropertyChanged != null)
                {
                    _Name = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
                else
                    _Name = value;
            }
        }

        private double _Stance;
        public double Stance
        {
            get
            {
                return _Stance;
            }
            set
            {
                if (!Equals(_Stance, value) && PropertyChanged != null)
                {
                    _Stance = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Stance"));
                }
                else
                    _Stance = value;
            }
        }

        private short _Health;
        public short Health
        {
            get
            {
                return _Health;
            }
            set
            {
                if (!Equals(_Health, value) && PropertyChanged != null)
                {
                    _Health = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Health"));
                }
                else
                    _Health = value;
            }
        }

        private short _Food;
        public short Food
        {
            get
            {
                return _Food;
            }
            set
            {
                if (!Equals(_Food, value) && PropertyChanged != null)
                {
                    _Food = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Food"));
                }
                else
                    _Food = value;
            }
        }

        /// <summary>
        /// The ticks until the food level will be reduced.
        /// </summary>
        public int FoodTimer { get; set; }

        public float FoodExaustionLevel { get; set; }

        private float _FoodSaturation;
        public float FoodSaturation
        {
            get
            {
                return _FoodSaturation;
            }
            set
            {
                if (!Equals(_FoodSaturation, value) && PropertyChanged != null)
                {
                    _FoodSaturation = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FoodSaturation"));
                }
                else
                    _FoodSaturation = value;
            }
        }

        internal List<Vector3> LoadedColumns { get; set; }

        private GameMode _GameMode;
        public GameMode GameMode
        {
            get
            {
                return _GameMode;
            }
            set
            {
                if (!Equals(_GameMode, value) && PropertyChanged != null)
                {
                    _GameMode = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("GameMode"));
                }
                else
                    _GameMode = value;
            }
        }

        private int _MapLoadRadius;
        public int MapLoadRadius
        {
            get
            {
                return _MapLoadRadius;
            }
            set
            {
                if (!Equals(_MapLoadRadius, value) && PropertyChanged != null)
                {
                    _MapLoadRadius = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ChunkLoadRadius"));
                }
                else
                    _MapLoadRadius = value;
            }
        }

        private int _SelectedSlot;
        public int SelectedSlot
        {
            get
            {
                return _SelectedSlot;
            }
            set
            {
                if (!Equals(_SelectedSlot, value) && PropertyChanged != null)
                {
                    _SelectedSlot = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedSlot"));
                }
                else
                    _SelectedSlot = value;
            }
        }

        public Slot SelectedItem 
        {
            get
            {
                return Inventory[SelectedSlot];
            }
        }

        /// <summary>
        /// Ticks until the chunk load radius increases
        /// </summary>
        internal int MapLoadRadiusDueTime { get; set; }

        private Slot[] _Inventory;
        public Slot[] Inventory
        {
            get
            {
                return _Inventory;
            }
            set
            {
                if (!Equals(_Inventory, value) && PropertyChanged != null)
                {
                    _Inventory = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Inventory"));
                }
                else
                    _Inventory = value;
            }
        }

        /// <summary>
        /// Is the bed occupied
        /// </summary>
        private Vector3 _OccupiedBed;
        public Vector3 OccupiedBed
        {
            get
            {
                return _OccupiedBed;
            }
            set
            {
                if (!Equals(_OccupiedBed, value) && PropertyChanged != null)
                {
                    _OccupiedBed = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("OccupiedBed"));
                }
                else
                    _OccupiedBed = value;
            }
        }


        /// <summary>
        /// Gets the item currently held
        /// </summary>
        public Slot InHand
        {
            get
            {
                return Inventory[SelectedSlot];
            }
        }

        private int _XpTotal;
        public int XpTotal
        {
            get
            {
                return _XpTotal;
            }
            set
            {
                if (!Equals(_XpTotal, value) && PropertyChanged != null)
                {
                    _XpTotal = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("XpTotal"));
                }
                else
                    _XpTotal = value;
            }
        }

        private int _XpLevel;
        public int XpLevel
        {
            get
            {
                return _XpLevel;
            }
            set
            {
                if (!Equals(_XpLevel, value) && PropertyChanged != null)
                {
                    _XpLevel = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("XpLevel"));
                }
                else
                    _XpLevel = value;
            }
        }

        public float XpPercentage
        {
            get
            {
                int currentRequired = (int)Math.Pow(1.75 * (XpLevel + 1), 2) + (5 * (XpLevel + 1));
                int nextRequired = (int)Math.Pow(1.75 * XpLevel, 2) + (5 * XpLevel);
                return currentRequired / nextRequired;
            }
        }

        public override void Tick(World world)
        {
            Block upper = world.GetBlock(Location + Vector3.Up);
            Block lower = world.GetBlock(Location);

            ScheduledEntityUpdate nextUpdate = null;

            if (ScheduledUpdateManager.Updates.Where(u => u is ScheduledEntityUpdate &&
                (u as ScheduledEntityUpdate).Entity.ID == this.ID).Count() != 0)
            {
                nextUpdate = (ScheduledEntityUpdate)ScheduledUpdateManager.Updates.Where(u => u is ScheduledEntityUpdate &&
                    (u as ScheduledEntityUpdate).Entity.ID == this.ID).First();
            }

            if (nextUpdate == null)
            {
                if (upper is NetherPortalBlock || lower is NetherPortalBlock)
                    ScheduledUpdateManager.AddUpdate(100, new ScheduledEntityUpdate(this, world));
            }
            else
            {
                if (!(upper is NetherPortalBlock || lower is NetherPortalBlock))
                    ScheduledUpdateManager.Updates.Remove(nextUpdate);
            }
            if (upper is EndPortalBlock || lower is EndPortalBlock)
                this.Dimension = Model.Dimension.TheEnd;
            base.Tick(world);
        }

        public override void ScheduledUpdate(World world)
        {
            this.Dimension = Model.Dimension.Nether;
            base.ScheduledUpdate(world);
        }

        protected override void FirePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(sender, e);
        }

        /// <summary>
        /// Fires when a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public override byte TypeID
        {
            get { return 0; }
        }

        public override Cuboid CollisionBox
        {
            get { return new Cuboid(this.Location, new Vector3(0.6, 1.8, 0.6)); }
        }
    }
}
