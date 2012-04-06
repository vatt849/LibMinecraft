using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// The Dirt Block
    /// </summary>
    /// <remarks></remarks>
    public class DirtBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 3; }
        }
    }
}
