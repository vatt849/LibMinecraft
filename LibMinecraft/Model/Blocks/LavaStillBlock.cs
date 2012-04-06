﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    public class LavaStillBlock : Block
    {
        public override byte BlockID
        {
            get { return 11; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Fluid; }
        }
    }
}
