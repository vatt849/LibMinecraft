using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Items
{
    /// <summary>
    /// A Bed (ID = 355)
    /// </summary>
    /// <remarks></remarks>
    public class BedItem : Item
    {
        /// <summary>
        /// The ID for this Item (355)
        /// </summary>
        /// <remarks></remarks>
        public override short ItemID
        {
            get { return 355; }
        }


        public override void PlayerUseItem(World world, Entities.PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            BedBlock b = new BedBlock();
            b.BlockPlaced(world, location, targetBlock, facing, player);
            world.SetBlock(location, b);
        }
    }
}
