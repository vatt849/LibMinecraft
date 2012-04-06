using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Bedrock block (ID = 7)
    /// </summary>
    public class BedrockBlock : Block
    {
        /// <summary>
        /// This block's ID (7)
        /// </summary>
        public override byte BlockID
        {
            get { return 0x07; }
        }
    }
}
