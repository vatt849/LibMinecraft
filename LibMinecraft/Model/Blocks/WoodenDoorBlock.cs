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
    /// A Wooden Door block (ID = 64)
    /// </summary>
    /// <remarks></remarks>
    public class WoodenDoorBlock : DoorBlock
    {
        /// <summary>
        /// The Block ID for this block (64)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 64; }
        }


        public WoodenDoorBlock(byte facing, bool Top, PlayerEntity placer) : base(facing, Top, placer) { }
        public WoodenDoorBlock(byte facing, PlayerEntity placer) : base(facing, placer) { }
        public WoodenDoorBlock() { }

        public override bool BlockRightClicked(World world, Vector3 position, PlayerEntity clickedBy)
        {
            unchecked
            {
                if ((Metadata & (byte)Doors.TopHalf) == (byte)Doors.TopHalf) // Top
                {
                    Block b = world.GetBlock(new Vector3(position.X, position.Y - 1, position.Z));
                    if (b is WoodenDoorBlock)
                    {
                        b.Metadata ^= (byte)Doors.Open;
                        world.SetBlock(new Vector3(position.X, position.Y - 1, position.Z), b);
                    }
                }
                else
                {
                    Block b = world.GetBlock(new Vector3(position.X, position.Y + 1, position.Z));
                    if (b is WoodenDoorBlock)
                    {
                        b.Metadata ^= (byte)Doors.Open;
                        world.SetBlock(new Vector3(position.X, position.Y + 1, position.Z), b);
                    }
                }
                Metadata ^= (byte)Doors.Open;
                world.SetBlock(position, this);
                MultiplayerServer.This.EnqueueToAllClientsExcept(new SoundOrParticleEffectPacket(SoundOrParticleEffect.DoorToggle,
                    position, 0), clickedBy);
                return false;
            }
        }
    }
}
