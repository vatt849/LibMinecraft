using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    /// <summary>
    /// A Brewing Stand item (ID = 379)
    /// </summary>
    /// <remarks></remarks>
    public class BrewingStandItem : Item
    {
        /// <summary>
        /// The ID for this Item (379)
        /// </summary>
        /// <remarks></remarks>
        public override short ItemID
        {
            get { return 379; }
        }
        
        /// <summary>
        /// The event or method fired when a BrewingStandItem is used.
        /// </summary>
        /// <param name="world">The world the brewing stand is used in.</param>
        /// <param name="player">The player who used the block.</param>
        /// <param name="location">The location of use.</param>
        /// <param name="targetBlock">The block which was targeted by the player.</param>
        /// <param name="facing">The direction in which the player was facing.</param>
        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            world.SetBlock(location, new BrewingStandBlock());
        }
    }
}
