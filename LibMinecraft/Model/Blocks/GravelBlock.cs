using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Gravel block (ID = 13)
    /// </summary>
    /// <remarks></remarks>
    public class GravelBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (13)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 13; }
        }

        public override void BlockUpdate(World world, Vector3 position)
        {
            Block b = world.GetBlock(position + Vector3.Down);
            if (b is AirBlock ||
                b is WaterFlowingBlock ||
                b is WaterStillBlock ||
                b is LavaFlowingBlock ||
                b is LavaStillBlock)
            {
                world.AddEntity(new FallingGravelEntity(position));
                world.SetBlock(position, new AirBlock());
            }
        }
    }
}
