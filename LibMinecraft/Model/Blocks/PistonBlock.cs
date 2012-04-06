using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Piston block (ID = 33)
    /// </summary>
    /// <remarks></remarks>
    public class PistonBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (33)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 33; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            this.Metadata = MathHelper.DirectionByRotation((PlayerEntity)placedBy, position, true);
            return true;
        }

        /// <summary>
        /// Pistons are cube solids.
        /// NOTE: THIS IS ONLY WHEN RETRACTED.
        /// </summary>
        public override BlockOpacity Transparent
        {
            get
            {
                return BlockOpacity.CubeSolid;
            }
        }
    }
}
