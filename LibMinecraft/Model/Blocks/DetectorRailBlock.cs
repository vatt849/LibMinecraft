using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Detector Rail Block (ID = 28)
    /// </summary>
    /// <remarks></remarks>
    public class DetectorRailBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (28)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 28; }
        }

        /// <summary>
        /// Returns whether or not a block requires support.
        /// DetectorRailBlock's require support.
        /// </summary>
        public override bool RequiresSupport
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the opacirty of a block.
        /// A Detector Rail Block is a NonSolidMechanism
        /// </summary>
        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolidMechanism; }
        }
    }
}

