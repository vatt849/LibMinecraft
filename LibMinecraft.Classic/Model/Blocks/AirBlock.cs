using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// An Air block (ID = 0)
    /// </summary>
    /// <remarks></remarks>
    public class AirBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 0x00; }
        }
    }
}
