using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// The base class for Stair blocks
    /// </summary>
    /// <remarks></remarks>
    public abstract class StairsBlock : Block
    {
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entities.Entity placedBy)
        {
            Metadata = MathHelper.DirectionByRotationFlat(placedBy);
            switch ((Directions)Metadata)
            {
                case Directions.North:
                    Metadata = (byte)StairDirections.North;
                    break;
                case Directions.South:
                    Metadata = (byte)StairDirections.South;
                    break;
                case Directions.West:
                    Metadata = (byte)StairDirections.West;
                    break;
                case Directions.East:
                    Metadata = (byte)StairDirections.East;
                    break;
            }
            if (blockClicked.Equals(position + Vector3.Up))
                this.Metadata |= 4;
            return true;
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonCubeSolid; }
        }
    }
}
