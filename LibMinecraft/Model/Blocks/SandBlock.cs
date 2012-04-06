using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Sand block (ID = 12)
    /// </summary>
    /// <remarks></remarks>
    public class SandBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (12)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 12; }
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
                world.AddEntity(new FallingSandEntity(position));
                world.SetBlock(position, new AirBlock());
            }
        }
    }
}
