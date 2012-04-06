using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// Sapling ID=6
    /// </summary>
    public class SaplingBlock:Block
    {
        /// <summary>
        /// The Block ID for this block (6)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 6; }
        }
        internal Tree SaplingType { get; set; }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }
    }
}
