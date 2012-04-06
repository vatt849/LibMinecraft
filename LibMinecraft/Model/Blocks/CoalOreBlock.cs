using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Coal Ore Block (ID = 16)
    /// </summary>
    /// <remarks></remarks>
    public class CoalOreBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (16)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 16; }
        }
    }
}
