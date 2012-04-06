using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// Log Block ID=17
    /// </summary>
    public class LogBlock:Block
    {
        /// <summary>
        /// The Block ID for this block (17)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 17; }
        }

        internal Tree LogType { get; set; }
    }
}
