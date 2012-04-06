using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Seed block (ID = 59)
    /// </summary>
    /// <remarks></remarks>
    public class SeedBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (59)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 59; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entities.Entity placedBy)
        {
            if (this.Metadata != 0x7)
                ScheduleUpdate(MultiplayerServer.Random.Next(500, 2000), position, world);
            return true;
        }

        public override void ScheduledUpdate(World world, Vector3 position)
        {
            if (this.Metadata != 0x7)
            {
                this.Metadata++;
                ScheduleUpdate(MultiplayerServer.Random.Next(500, 2000), position, world);
            }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }
    }
}
