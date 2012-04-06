using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    /// <summary>
    /// A Cauldron item (ID = 380)
    /// </summary>
    /// <remarks></remarks>
    public class CauldronItem : Item
    {
        /// <summary>
        /// The ID for this Item (380)
        /// </summary>
        /// <remarks></remarks>
        public override short ItemID
        {
            get { return 380; }
        }

        /// <summary>
        /// When the player uses an the Cauldron.
        /// </summary>
        /// <param name="world">The world in which the Cauldron resides.</param>
        /// <param name="player">The player who uses the item</param>
        /// <param name="location">The location where the item was used.</param>
        /// <param name="targetBlock">The target block of which was aimed at by the player during the Cauldron's time of use.</param>
        /// <param name="facing">The direciton in which the player was facing.</param>
        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            world.SetBlock(location, new CauldronBlock());
        }
    }
}
