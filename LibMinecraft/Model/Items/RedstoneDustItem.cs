using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    public class RedstoneDustItem : Item
    {
        public override short ItemID
        {
            get { return 331; }
        }

        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            RedstoneWireBlock b = new RedstoneWireBlock();
            b.BlockPlaced(world, location, targetBlock, facing, player);
            world.SetBlock(location, b);
        }
    }
}
