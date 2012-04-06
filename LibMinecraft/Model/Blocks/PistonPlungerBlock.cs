using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Piston Plunger block (ID = 33)
    /// This block is the block on the end of a piston which moves forward and backwards
    /// </summary>
    /// <remarks></remarks>
    public class PistonPlungerBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (33)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 33; }
        }
        public override byte Metadata
        {
            get
            {
                return base.Metadata;
            }
            set
            {
                base.Metadata = value;
            }
        }
        public PistonPlungerBlock()
        {
        }
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            this.Metadata = MathHelper.DirectionByRotationFlat((PlayerEntity)placedBy, true);
            return true;
        }
    }

}
