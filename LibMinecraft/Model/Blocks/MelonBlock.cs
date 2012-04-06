using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// Melon Block ID=103
    /// </summary>
    public class MelonBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (103)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 103; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }
    }
}
