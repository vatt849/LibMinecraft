using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// A Wooden plank block (ID = 5)
    /// </summary>
    /// <remarks></remarks>
    public class WoodenPlankBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 0x05; }
        }
    }
}
