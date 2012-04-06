using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// Pumkpin Stem ID=106
    /// </summary>
    public class MelonStemBlock : Block
    {
        public override byte BlockID
        {
            get { return 105; }
        }

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
