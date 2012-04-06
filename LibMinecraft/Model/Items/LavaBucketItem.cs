using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    public class LavaBucketItem : Item
    {
        public override short ItemID
        {
            get { return 327; }
        }

        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            world.SetBlock(location, new LavaFlowingBlock());
        }
    }
}
