using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Grass block (ID = 2)
    /// </summary>
    /// <remarks></remarks>
    public class GrassBlock : Block
    {
        static Block[] AdjacentBlocks = new Block[24]; // Maximum number of applicable blocks

        /// <summary>
        /// The Block ID for this block (2)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 0x02; }
        }

        public override void BlockUpdate(World world, Vector3 position)
        {
            ScheduleUpdate(MultiplayerServer.Random.Next(100, 1000), position, world);
            base.BlockUpdate(world, position);
        }

        public override void ScheduledUpdate(World world, Vector3 position)
        {
            Block b = world.GetBlock(position + Vector3.Up);
            if (b.Transparent == BlockOpacity.Opaque || b.Transparent == BlockOpacity.CubeSolid)
                world.SetBlock(position, new DirtBlock());
        }
    }
}
