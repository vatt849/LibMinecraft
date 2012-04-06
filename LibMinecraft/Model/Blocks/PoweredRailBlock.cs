using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Powered Rail block (ID = 27)
    /// </summary>
    /// <remarks></remarks>
    public class PoweredRailBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (27)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 27; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }
    }
}

