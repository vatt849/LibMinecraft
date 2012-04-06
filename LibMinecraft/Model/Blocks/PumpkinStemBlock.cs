﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// Pumpkin Stem ID=106
    /// </summary>
    public class PumpkinStemBlock : Block
    {
        public override byte BlockID
        {
            get { return 104; }
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
