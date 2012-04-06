using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// An Inactive Repeater block (ID = 93)
    /// </summary>
    /// <remarks></remarks>
    public class RepeaterBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (93)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 93; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonCubeSolid; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }

    }
}
