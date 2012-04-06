using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Brewing Stand block (ID = 117)
    /// </summary>
    public class BrewingStandBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (117)
        /// </summary>
        public override byte BlockID
        {
            get { return 117; }
        }

        /// <summary>
        /// Called when this block is right clicked
        /// Stops more blocks from being placed on it
        /// </summary>
        public override bool BlockRightClicked(World world, Vector3 position, Entities.PlayerEntity clickedBy)
        {
            return false;
        }

        /// <summary>
        /// Returns whether or not a block requires support.
        /// Brewing Stand requires support.
        /// </summary>
        public override bool RequiresSupport
        {
            get { return true; }
        }
    }
}
