using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Items;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Tall Grass block (ID 31)
    /// </summary>
    /// <remarks></remarks>
    public class TallGrassBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (31)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 0x1F; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }

        private byte internalMetadata;

        public override byte Metadata
        {
            get
            {
                return internalMetadata;
            }
        }

        public TallGrassBlock()
        {
        }

        public TallGrassBlock(byte Metadata)
        {
            this.internalMetadata = Metadata;
        }

        public override double GetTimeToBreak(Item tool)
        {
            return -1;
        }


    }
}
