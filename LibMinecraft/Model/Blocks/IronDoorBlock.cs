using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;
using LibMinecraft.Model.Packets;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// An Iron Door block (ID = 71)
    /// </summary>
    /// <remarks></remarks>
    public class IronDoorBlock : DoorBlock
    {
        /// <summary>
        /// The Block ID for this block (71)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 71; }
        }
        
        public IronDoorBlock(byte facing, bool Top, PlayerEntity placer) : base(facing, Top, placer) { }
        public IronDoorBlock(byte facing, PlayerEntity placer) : base(facing, placer) { }
        public IronDoorBlock() { }
    }
}
