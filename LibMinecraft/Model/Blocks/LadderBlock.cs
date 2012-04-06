using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Ladder block (ID = 65)
    /// </summary>
    /// <remarks></remarks>
    public class LadderBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (65)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 65; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entities.Entity placedBy)
        {
            this.Metadata = MathHelper.DirectionByRotationFlat((PlayerEntity)placedBy, true);
            return true;
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonCubeSolid; }
        }

        public override Vector3 SupportingBlockDirection
        {
            get
            {
                switch (this.Metadata)
                {
                    case 2:
                        return Vector3.South;
                    case 3:
                        return Vector3.North;
                    case 4:
                        return Vector3.East;
                    case 5:
                        return Vector3.West;
                    default:
                        return Vector3.North;
                }
            }
        }

        public override bool RequiresSupport
        {
            get { return true; }
        }
    }
}
