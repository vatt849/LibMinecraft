using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Vine block (ID = 106)
    /// </summary>
    public class VinesBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (106)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 106; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }
    }
}
