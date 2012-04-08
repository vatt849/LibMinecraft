using System;
using System.Collections.Generic;
using System.Linq;
using LibMinecraft.Server;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Bed block (ID = 26)
    /// </summary>
    public class BedBlock : Block
    {
        /// <summary>
        /// This block's ID
        /// </summary>
        public override byte BlockID
        {
            get { return 26; }
        }

        /// <summary>
        /// Called when this block is placed in the world
        /// </summary>
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entities.Entity placedBy)
        {
            // Get facing
            facing = MathHelper.DirectionByRotationFlat(placedBy);
            Vector3 secondHalf = position.Clone();

            byte occupied = 0x4;

            switch ((Directions)facing)
            {
                case Directions.North:
                    facing = (byte)Bed.North;
                    secondHalf.X--;
                    break;
                case Directions.East:
                    facing = (byte)Bed.East;
                    secondHalf.Z--;
                    break;
                case Directions.West:
                    facing = (byte)Bed.West;
                    secondHalf.Z++;
                    break;
                case Directions.South:
                    facing = (byte)Bed.South;
                    secondHalf.X++;
                    break;
            }
            this.Metadata = facing;
            Block other = world.GetBlock(secondHalf);
            if (!(other is AirBlock))
                return false;
            other = new BedBlock();
            other.Metadata = (byte)(facing | 0x08);
            world.SetBlock(secondHalf, other);

            return true;
        }

        /// <summary>
        /// Called when this block is placed in the world.
        /// Disables this block from being placed on.
        /// </summary>
        public override bool BlockRightClicked(World world, Vector3 position, Entities.PlayerEntity clickedBy)
        {
            if ((world.Level.Time % 24000) > 12000) //If within proper time period
            {
                if ((this.Metadata & 0x8) != 0x8) //If the peice isn't the headboard
                {
                    //Do shit in here
                }

                if ((this.Metadata & 0x4) != 0x4 && clickedBy.OccupiedBed == null)
                { // If it isn't occupied
                    this.Metadata = (byte)(this.Metadata & ~0x4);
                    this.Metadata = (byte)(this.Metadata | 0x4); //Occupy it

                    clickedBy.OccupiedBed = position;

                    world.SetBlock(position, this);
                }
                else
                {
                    //clickedBy.SendChat("Sorry, the bed you are currently trying to use is occupied.");
                }
            }
            else { }
               //(clickedBy).SendChat("Sorry, beds can only be used during the night.");
            return false;
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonCubeSolid; }
        }
    }
}
