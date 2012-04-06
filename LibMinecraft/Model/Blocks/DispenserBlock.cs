using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// The Dispenser block
    /// </summary>
    /// <remarks></remarks>
    public class DispenserBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 23; }
        }

        /// <summary>
        /// Called when this block is placed
        /// </summary>
        /// <param name="world">The world it was placed in</param>
        /// <param name="position">The position it was placed at</param>
        /// <param name="blockClicked">The location of the block left clicked upon placing</param>
        /// <param name="facing">The facing of the placement</param>
        /// <param name="placedBy">The entity who placed the block</param>
        /// <returns>False to override placement</returns>
        /// <remarks></remarks>
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entities.Entity placedBy)
        {
            this.Metadata = MathHelper.DirectionByRotationFlat((PlayerEntity)placedBy, true);
            return true;
        }

        /// <summary>
        /// Called when this block is right clicked by a player.
        /// </summary>
        /// <param name="world">The world in which the event occured</param>
        /// <param name="position">The location of the block being clicked</param>
        /// <param name="clickedBy">The player who clicked the block</param>
        /// <returns>False to override the default action (block placement)</returns>
        /// <remarks></remarks>
        public override bool BlockRightClicked(World world, Vector3 position, PlayerEntity clickedBy)
        {
            return false;
        }
    }
}

