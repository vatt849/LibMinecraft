using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Nether Wart block (ID = 115)
    /// </summary>
    /// <remarks></remarks>
    public class NetherWartBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (115)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 115; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            ScheduleUpdate(MultiplayerServer.Random.Next(500, 2000), position, world);
            return true;
        }

        public override void ScheduledUpdate(World world, Vector3 position)
        {
            if (this.Metadata < 3)
            {
                this.Metadata++;
                ScheduleUpdate(MultiplayerServer.Random.Next(500, 2000), position, world);
            }
        }


        public override bool RequiresSupport
        {
            get { return true; }
        }
    }
}
