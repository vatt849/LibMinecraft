using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Wooden Stairs block (ID = 53)
    /// </summary>
    /// <remarks></remarks>
    public class WoodenStairsBlock : StairsBlock
    {
        /// <summary>
        /// The Block ID for this block (53)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 53; }
        }
    }
}
