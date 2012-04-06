using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Furnace block (ID = 61)
    /// </summary>
    /// <remarks></remarks>
    public class FurnaceBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (61)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 61; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            this.Metadata = MathHelper.DirectionByRotationFlat((PlayerEntity)placedBy, true);
            return true;
        }

        public override bool BlockRightClicked(World world, Vector3 position, PlayerEntity clickedBy)
        {
            return false;
        }
    }
}
