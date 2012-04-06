using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    /// <summary>
    /// A Bucket item (ID = 325)
    /// </summary>
    /// <remarks></remarks>
    public class BucketItem : Item
    {
        /// <summary>
        /// The ID for this Item (325)
        /// </summary>
        /// <remarks></remarks>
        public override short ItemID
        {
            get { return 325; }
        }

        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            Block b = world.GetBlock(location);
            if (b is WaterStillBlock || b is WaterFlowingBlock || b is LavaStillBlock || b is LavaFlowingBlock)
            {
                world.SetBlock(location, new AirBlock());
                if (player.GameMode == GameMode.Survival)
                {
                    // TODO: Inventory
                }
            }
        }
    }
}
