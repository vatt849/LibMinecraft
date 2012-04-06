﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Note Block (ID = 25)
    /// </summary>
    /// <remarks></remarks>
    public class NoteBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (25)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 25; }
        }
    }
}

