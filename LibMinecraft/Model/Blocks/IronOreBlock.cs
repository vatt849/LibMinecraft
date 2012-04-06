using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// An Iron Ore block (ID = 15)
    /// </summary>
    /// <remarks></remarks>
    public class IronOreBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (15)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 15; }
        }
    }
}
