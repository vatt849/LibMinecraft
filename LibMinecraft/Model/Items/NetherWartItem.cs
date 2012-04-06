using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    public class NetherWartItem : Item
    {
        public override short ItemID
        {
            get { return 372; }
        }

        public override void PlayerUseItem(World world, PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            if (world.GetBlock(targetBlock) is SoulSandBlock)
            {
                NetherWartBlock b = new NetherWartBlock();
                if (b.BlockPlaced(world, location, targetBlock, facing, player))
                    world.SetBlock(location, b);
            }
        }
    }
}
