using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// An Inactive Redstone Torch (ID = 75)
    /// </summary>
    /// <remarks></remarks>
    public class RedstoneTorchBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (75)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 75; }
        }

        public override void BlockUpdate(World world, Vector3 position)
        {
            if (this.Metadata != 5)
            {
                
            }

            base.BlockUpdate(world, position);
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            switch (facing)
            {
                case 1:
                    Metadata = 5;
                    break;
                case 2:
                    Metadata = 4;
                    break;
                case 3:
                    Metadata = 3;
                    break;
                case 4:
                    Metadata = 2;
                    break;
                case 5:
                    Metadata = 1;
                    break;
                default:
                    return false;
            }
            return true;
        }

        public override Vector3 SupportingBlockDirection
        {
            get
            {
                switch (this.Metadata)
                {
                    case 1:
                        return Vector3.West;
                    case 2:
                        return Vector3.East;
                    case 3:
                        return Vector3.North;
                    case 4:
                        return Vector3.South;
                    case 5:
                        return Vector3.Down;
                    default:
                        return Vector3.Down;
                }
            }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolidMechanism; }
        }

    }
}
