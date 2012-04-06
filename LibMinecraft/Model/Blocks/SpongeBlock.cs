using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Sponge block (ID = 19)
    /// </summary>
    /// <remarks></remarks>
	public class SpongeBlock : Block
	{
        /// <summary>
        /// The Block ID for this block (19)
        /// </summary>
        /// <remarks></remarks>
		public override byte BlockID
		{
            get {return 19;}
		}
	}
}
