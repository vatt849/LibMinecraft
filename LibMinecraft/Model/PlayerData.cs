using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model
{
    public class PlayerData
    {
        public bool OnGround { get; set; }
        public bool Sleeping { get; set; }
        public short AirTime { get; set; }
        public short Fire { get; set; }
        public short Health { get; set; }
        public short SleepTimer { get; set; }
        public Dimension Dimension { get; set; }
        public int Food { get; set; }
        public int FoodTickTimer { get; set; }
        public GameMode GameMode { get; set; }
        public int XpLevel { get; set; }
        public int XpTotal { get; set; }
        public float FallDistance { get; set; }
        public float FoodExhastionLevel { get; set; }
        public float FoodSaturationLevel { get; set; }

        public Slot[] Inventory { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }
}
