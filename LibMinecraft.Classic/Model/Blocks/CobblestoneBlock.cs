using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// A Cobblestone block (ID = 4)
    /// </summary>
    /// <remarks></remarks>
    public class CobblestoneBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 0x04; }
        }
    }
}
