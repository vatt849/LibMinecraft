using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Dandelion Block (ID=37)
    /// </summary>
    public class DandelionBlock : Block
    {
        /// <summary>
        /// Returns a block's ID.
        /// Dandelion (ID=37)
        /// </summary>
        public override byte BlockID
        {
            get { return 37; }
        }

        /// <summary>
        /// Returns a blocks transparency.
        /// A Dandelion is a plant.
        /// </summary>
        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }

        /// <summary>
        /// Returns whether or not a block requires a supporting block.
        /// Dandelions require support.
        /// </summary>
        public override bool RequiresSupport
        {
            get { return true; }
        }
    }
}

