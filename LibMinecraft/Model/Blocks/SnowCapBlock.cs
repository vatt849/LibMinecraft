﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Snow Cap block (ID = 78)
    /// </summary>
    /// <remarks></remarks>
    public class SnowCapBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (78)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 78; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }

        public override BlockOpacity Transparent
        {
            get
            {
                return BlockOpacity.NonSolid;
            }
        }
    }
}
