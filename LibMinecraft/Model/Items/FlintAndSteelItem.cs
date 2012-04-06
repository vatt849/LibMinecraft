using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    public class FlintAndSteelItem : Item
    {
        public override short ItemID
        {
            get { return 259; }
        }

        public override void PlayerUseItem(World world, PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            FireBlock b = new FireBlock();
            if (b.BlockPlaced(world, location, targetBlock, facing, player))
                world.SetBlock(location, b);
        }
    }
}
