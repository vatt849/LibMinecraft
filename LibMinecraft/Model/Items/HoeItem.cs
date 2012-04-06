using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Items
{
    public abstract class HoeItem : Item
    {
        public override void PlayerUseItem(World world, PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            Block b = world.GetBlock(targetBlock);
            if (b is DirtBlock || b is GrassBlock)
                world.SetBlock(targetBlock, new FarmlandBlock());
        }
    }
}
