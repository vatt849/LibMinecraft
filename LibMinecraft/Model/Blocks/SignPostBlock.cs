using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibNbt;
using LibMinecraft.Model.Packets;
using System.IO;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// Class of the Sign Post Block (ID = 63)
    /// </summary>
    public class SignPostBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (63)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 63; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolid; }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }

        public override BlockData AdditionalData 
        {
            get
            {
                return Data;
            }
        }

        public SignData Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignPostBlock"/> class.
        /// </summary>
        public SignPostBlock()
        {
            Data = new SignData();
        }

        /// <summary>
        /// Called when this block is placed
        /// </summary>
        /// <param name="world">The world it was placed in</param>
        /// <param name="position">The position it was placed at</param>
        /// <param name="blockClicked">The location of the block left clicked upon placing</param>
        /// <param name="facing">The facing of the placement</param>
        /// <param name="placedBy">The entity who placed the block</param>
        /// <returns>
        /// False to override placement
        /// </returns>
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            double rotation = placedBy.Rotation.X + 180 % 360;
            if (rotation < 0)
                rotation += 360;

            this.Metadata = (byte)(rotation / 22.5);
            return true;
        }

        public class SignData : BlockData
        {
            public string[] Text { get; set; }

            public SignData()
            {
                Text = new string[] { "", "", "", "" };
            }

            public override byte BlockID
            {
                get
                {
                    return 63;
                }
            }

            public override byte[] Serialize()
            {
                return Packet.MakeString(Text[0]).Concat(
                       Packet.MakeString(Text[1])).Concat(
                       Packet.MakeString(Text[2])).Concat(
                       Packet.MakeString(Text[3])).ToArray();
            }

            public override void Deseralize(byte[] Data)
            {
                MemoryStream s = new MemoryStream(Data);
                this.Text[0] = Packet.ReadString(s);
                this.Text[1] = Packet.ReadString(s);
                this.Text[2] = Packet.ReadString(s);
                this.Text[3] = Packet.ReadString(s);
            }

            public override NbtFile WriteNBT()
            {
                throw new NotImplementedException();
            }

            public override NbtFile ReadNBT()
            {
                throw new NotImplementedException();
            }




            public virtual bool RequiresSupport
            {
                get { return true; }
            }
        }
    }
}
