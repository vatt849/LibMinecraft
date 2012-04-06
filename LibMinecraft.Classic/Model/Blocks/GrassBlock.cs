using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// A Grass block (ID = 2)
    /// </summary>
    /// <remarks></remarks>
    public class GrassBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 0x02; }
        }
    }
}
