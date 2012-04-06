using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    public class IronDoorItem : Item
    {
        public override short ItemID
        {
            get { return 330; }
        }
        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            Block bottom = world.GetBlock(location);
            Block top = world.GetBlock(new Vector3(location.X, location.Y + 1, location.Z));
            if (bottom is AirBlock && top is AirBlock)
            {
                world.SetBlock(location, new IronDoorBlock(facing, player));
                world.SetBlock(new Vector3(location.X, location.Y + 1, location.Z), new IronDoorBlock(facing, true, player));
            }
        }
    }
}
