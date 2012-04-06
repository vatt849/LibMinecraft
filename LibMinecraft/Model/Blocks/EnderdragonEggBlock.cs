using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// Ender dragon egg ID=122
    /// </summary>
    public class EnderDragonEggBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 122; }
        }

        /// <summary>
        /// Called when an action in the world causes this block to update.
        /// </summary>
        /// <param name="world">The world the update occured in</param>
        /// <param name="position">The location of the updated block</param>
        /// <remarks></remarks>
        public override void BlockUpdate(World world, Vector3 position)
        {
            Block b = world.GetBlock(position + Vector3.Down);
            if (b is AirBlock ||
                b is WaterFlowingBlock ||
                b is WaterStillBlock ||
                b is LavaFlowingBlock ||
                b is LavaStillBlock)
            {
                world.AddEntity(new FallingEnderDragonEggEntity(position));
                world.SetBlock(position, new AirBlock());
            }
        }
    }
}
