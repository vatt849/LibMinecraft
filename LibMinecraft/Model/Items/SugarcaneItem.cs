using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    public class SugarcaneItem : Item
    {
        public override short ItemID
        {
            get { return 338; }
        }

        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            Block b = world.GetBlock(new Vector3(location.X, location.Y - 1, location.Z));
            if (b is SugarcaneBlock)
            {
                world.SetBlock(location, new SugarcaneBlock());
                return;
            }
            if (b is DirtBlock || b is GrassBlock || b.BlockID == 12) // Sand
            {
                // Look for water
                bool waterFound = false;
                for (int x = -1; x < 2; x++)
                    for (int z = -1; z < 2; z++)
                    {
                        if (world.GetBlock(new Vector3(location.X - x, location.Y - 1, location.Z - z)) is WaterFlowingBlock ||
                            world.GetBlock(new Vector3(location.X - x, location.Y - 1, location.Z - z)) is WaterStillBlock)
                        {
                            waterFound = true;
                        }
                    }
                if (waterFound)
                {
                    b = new SugarcaneBlock();
                    world.SetBlock(location, b);
                    b.BlockPlaced(world, location, targetBlock, facing, player);
                }
            }
        }
    }
}
