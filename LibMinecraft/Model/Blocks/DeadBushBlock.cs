using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Dead Bush Block (ID = 32)
    /// </summary>
    /// <remarks></remarks>
    public class DeadBushBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (32)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 32; }
        }

        /// <summary>
        /// Returns whether or not the block requires support.
        /// Dead Bushes require support.
        /// </summary>
        public override bool RequiresSupport
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the opacity of a block.
        /// A Dead Bush is a plant.
        /// </summary>
        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }
    }
}
