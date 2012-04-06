using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Items
{
    public class SignPostItem : Item
    {
        public override short ItemID
        {
            get { return 323; }
        }

        public override void PlayerUseItem(World world, PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            if (facing == (byte)Directions.Bottom)
                return;
            if (facing == (byte)Directions.Top)
            {
                SignPostBlock b = new SignPostBlock();
                if (b.BlockPlaced(world, location, targetBlock, facing, player))
                    world.SetBlock(location, b);
                return;
            }
            WallSignBlock c = new WallSignBlock();
            if (c.BlockPlaced(world, location, targetBlock, facing, player))
                world.SetBlock(location, c);
        }
    }
}
