using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Lilypad block (ID = 111)
    /// </summary>
    /// <remarks></remarks>
    public class LilyPadBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (111)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 111; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entities.Entity placedBy)
        {
            Vector3 surface = new Vector3(position.X, position.Y, position.Z);
            while (surface.Y < 128)
            {
                if (world.GetBlock(surface) is AirBlock)
                {
                    if (world.GetBlock(surface - new Vector3(0, 1, 0)) is WaterFlowingBlock ||
                        world.GetBlock(surface - new Vector3(0, 1, 0)) is WaterStillBlock)
                    {
                        world.SetBlock(surface, this);
                    }
                    return false;
                }
                else if (world.GetBlock(surface) is WaterFlowingBlock ||
                    world.GetBlock(surface) is WaterStillBlock)
                {
                    surface.Y++;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
