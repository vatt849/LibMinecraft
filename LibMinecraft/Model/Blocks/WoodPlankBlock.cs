using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Wood Plank block (ID = 5)
    /// </summary>
    /// <remarks></remarks>
    public class WoodPlankBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (5)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 5; }
        }
    }
}