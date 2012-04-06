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
    /// Fence Gate ID=107
    /// </summary>
    public class FenceGateBlock : Block
    {
        /// <summary>
        /// Returns the block ID of the block.
        /// Fence Gate ID=107
        /// </summary>
        public override byte BlockID
        {
            get { return 107; }
        }

        /// <summary>
        /// When a block is placed at a specific location, esp. that of a fence, it is positioned accordingly.
        /// This method is an extension of the already in place BlockPlaced of the Block abstract class.
        /// </summary>
        /// <param name="world">The world of the block's placement.</param>
        /// <param name="position">The position of the block's placement.</param>
        /// <param name="blockClicked">Where the block was attempting to be placed.</param>
        /// <param name="facing">In which direction the the placer was facing.</param>
        /// <param name="placedBy">Whp the block was placed by.</param>
        /// <returns>Whether or not the block has been placed.</returns>
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            Metadata = (byte)((int)Math.Floor((double)((placedBy.Rotation.X * 4F) / 360F) + 0.5D) & 3);
            return true;
        }

        /// <summary>
        /// Returns the opacity of a block.
        /// Fence Gate is a NonCubeSolid.
        /// </summary>
        public override BlockOpacity Transparent
        {
            get
            {
                return BlockOpacity.NonCubeSolid;
            }
        }

        /// <summary>
        /// The event and method of which is fired when a specific FenceGateBlock is right clicked.
        /// </summary>
        /// <param name="world">The world in which the block resides.</param>
        /// <param name="position">The position of the block.</param>
        /// <param name="clickedBy">And the Entity clicker who clicked the block.</param>
        /// <returns></returns>
        public override bool BlockRightClicked(World world, Vector3 position, PlayerEntity clickedBy)
        {
            unchecked
            {
                byte oldDirection = (byte)(Metadata & 1);
                Metadata ^= (byte)FenceGate.Open;
                Metadata &= 0xFC;
                byte direction = (byte)((int)Math.Floor((double)((clickedBy.Rotation.X * 4F) / 360F) + 0.5D) & 3);
                direction &= 0xFE;
                direction |= oldDirection;
                Metadata |= direction;
                world.SetBlock(position, this);
                MultiplayerServer.This.EnqueueToAllClientsExcept(new SoundOrParticleEffectPacket(SoundOrParticleEffect.DoorToggle,
                    position, 0), clickedBy);
                return false;
            }
        }
    }
}
