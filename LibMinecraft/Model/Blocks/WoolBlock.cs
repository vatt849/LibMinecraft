using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Wool Block (ID = 35)
    /// </summary>
    /// <remarks></remarks>
    public class WoolBlock:Block
    {
        /// <summary>
        /// The Block ID for this block (35)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 35 ; }
        }

        public WoolBlock()
        {
        }

        public WoolBlock(Wool Color)
        {
            this.Metadata = (byte)Color;
        }
    }
}
