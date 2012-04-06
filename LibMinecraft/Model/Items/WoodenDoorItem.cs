using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Items
{
    public class WoodenDoorItem : Item
    {
        public override short ItemID
        {
            get { return 324; }
        }

        public override void PlayerUseItem(World world, PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            Block bottom = world.GetBlock(location);
            Block top = world.GetBlock(new Vector3(location.X, location.Y + 1, location.Z));
            if (bottom is AirBlock && top is AirBlock)
            {
                world.SetBlock(location, new WoodenDoorBlock(facing, player));
                world.SetBlock(new Vector3(location.X, location.Y + 1, location.Z), new WoodenDoorBlock(facing, true, player));
            }
        }
    }
}
