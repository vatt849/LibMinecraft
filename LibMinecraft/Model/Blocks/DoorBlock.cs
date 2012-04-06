using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;
using LibMinecraft.Model.Packets;

namespace LibMinecraft.Model.Blocks
{
    // TODO: Double doors
    /// <summary>
    /// The base class for IronDoorBlock and WoodenDoorBlock
    /// </summary>
    /// <remarks></remarks>
    public abstract class DoorBlock : Block
    {
        #region Properties
        /// <summary>
        /// Just the overide for the original MetaData property.
        /// </summary>
        public override byte Metadata
        {
            get
            {
                return base.Metadata;
            }
            set
            {
                base.Metadata = value;
            }
        }

        /// <summary>
        /// Returns the opacity of a block.
        /// DoorBlock is a NonCubeSolid.
        /// </summary>
        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonCubeSolid; }
        }

        /// <summary>
        /// Returns whether or not a block requires a supporting block in it's radius.
        /// DoorBlock requires support.
        /// </summary>
        public override bool RequiresSupport
        {
            get { return true; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// DoorBlock placing
        /// </summary>
        /// <param name="facing"></param>
        /// <param name="placer"></param>
        public DoorBlock(byte facing, PlayerEntity placer)
        {
            facing = MathHelper.DirectionByRotationFlat(placer);
            switch ((Directions)facing)
            {
                case Directions.East:
                    facing = (byte)Doors.NorthWest;
                    break;
                case Directions.West:
                    facing = (byte)Doors.SouthEast;
                    break;
                case Directions.North:
                    facing = (byte)Doors.SouthWest;
                    break;
                case Directions.South:
                    facing = (byte)Doors.NorthEast;
                    break;
            }
            Metadata = facing;
        }

        /// <summary>
        /// Doorblock constructor allowing specification of top
        /// </summary>
        /// <param name="facing">The direction</param>
        /// <param name="Top">The top of the door</param>
        /// <param name="placer">Who placed the block</param>
        public DoorBlock(byte facing, bool Top, PlayerEntity placer) : this(facing,placer)
        {
            Metadata |= Top ? (byte)Doors.TopHalf : (byte)0;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DoorBlock()
        {
        }

#endregion

        #region Events
        /// <summary>
        /// When the block is destroyed, delete the block above.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="position"></param>
        /// <param name="destroyedBy"></param>
        public override void BlockDestroyed(World world, Vector3 position, Entity destroyedBy)
        {
            if ((Metadata & (byte)Doors.TopHalf) == (byte)Doors.TopHalf) // Top
            {
                if (world.GetBlock(new Vector3(position.X, position.Y - 1, position.Z)) is WoodenDoorBlock)
                    world.SetBlock(new Vector3(position.X, position.Y - 1, position.Z), new AirBlock());
            }
            else
            {
                if (world.GetBlock(new Vector3(position.X, position.Y + 1, position.Z)) is WoodenDoorBlock)
                    world.SetBlock(new Vector3(position.X, position.Y + 1, position.Z), new AirBlock());
            }
        }
        #endregion
    }
}
