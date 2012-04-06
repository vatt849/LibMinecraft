using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// An Air block (ID = 0)
    /// </summary>
    public class AirBlock : Block
    {
        /// <summary>
        /// This block's ID (0)
        /// </summary>
        public override byte BlockID
        {
            get { return 0; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolid; }
        }
    }
}
