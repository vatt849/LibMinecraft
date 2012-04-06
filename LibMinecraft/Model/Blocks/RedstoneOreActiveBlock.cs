using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// An Active Redstone Ore block (ID = 74)
    /// </summary>
    /// <remarks></remarks>
    public class RedstoneOreActiveBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (74)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 74; }
        }
    }
}
