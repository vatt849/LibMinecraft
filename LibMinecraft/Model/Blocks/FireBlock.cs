using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// FireBlock (ID = 51)
    /// </summary>
    public class FireBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (51)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 51; }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolid; }
        }

        /// <summary>
        /// Returns whether or not a block requires a supporting block in it's radius.
        /// FireBlock requires support.
        /// </summary>
        public override bool RequiresSupport
        {
            get { return true; }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entities.Entity placedBy)
        {
            if (ValidatePortal(world, position))
            {
                world.EnableUpdates = false;
                for (int y = 1; y <= 3; y++)
                {
                    world.SetBlock(topLeft - new Vector3(0, y, 0), new NetherPortalBlock());
                    world.SetBlock(topRight - new Vector3(0, y, 0), new NetherPortalBlock());
                }
                world.EnableUpdates = true;
                return false;
            }
            return true;
        }

        Vector3 topLeft, topRight, outsideLeft, outsideRight;

        bool ValidatePortal(World world, Vector3 position)
        {
            // Check top
            int y;
            Block a, b, c, d;
            for (y = 1; y <= 3; y++)
            {
                b = world.GetBlock(position + new Vector3(0, y, 0));
                if (b is ObsidianBlock)
                    break;
                if (!(b is NetherPortalBlock || b is AirBlock || b is FireBlock))
                    return false;
            }

            if (world.GetBlock(position + new Vector3(2, y - 1, 0)) is ObsidianBlock)
            {
                topLeft = position + new Vector3(0, y, 0);
                topRight = topLeft + new Vector3(1, 0, 0);
                outsideLeft = position + new Vector3(-1, y, 0);
                outsideRight = position + new Vector3(2, y, 0);
            }
            else if (world.GetBlock(position + new Vector3(-2, y - 1, 0)) is ObsidianBlock)
            {
                topLeft = position + new Vector3(0, y, 0);
                topRight = topLeft - new Vector3(1, 0, 0);
                outsideLeft = position + new Vector3(1, y, 0);
                outsideRight = position + new Vector3(-2, y, 0);
            }
            else if (world.GetBlock(position + new Vector3(0, y - 1, 2)) is ObsidianBlock)
            {
                topLeft = position + new Vector3(0, y, 0);
                topRight = topLeft + new Vector3(0, 0, 1);
                outsideLeft = position + new Vector3(0, y, -1);
                outsideRight = position + new Vector3(0, y, 2);
            }
            else if (world.GetBlock(position + new Vector3(0, y - 1, -2)) is ObsidianBlock)
            {
                topLeft = position + new Vector3(0, y, 0);
                topRight = topLeft - new Vector3(0, 0, 1);
                outsideLeft = position + new Vector3(0, y, 1);
                outsideRight = position + new Vector3(0, y, -2);
            }
            else
                return false; // TODO: z-oriented portals

            Vector3 direction = Vector3.Right;

            // Check for air/fire/portal/obsidian blocks
            for (y = 1; y <= 3; y++)
            {
                a = world.GetBlock(topLeft - new Vector3(0, y, 0));
                b = world.GetBlock(topRight - new Vector3(0, y, 0));
                c = world.GetBlock(outsideLeft - new Vector3(0, y, 0));
                d = world.GetBlock(outsideRight - new Vector3(0, y, 0));
                if (!(a is AirBlock || a is FireBlock || a is NetherPortalBlock) ||
                    !(b is AirBlock || b is FireBlock || b is NetherPortalBlock) ||
                    !(c is ObsidianBlock) || !(d is ObsidianBlock))
                {
                    return false;
                }
            }

            return world.GetBlock(topLeft - new Vector3(0, 4, 0)) is ObsidianBlock &&
                   world.GetBlock(topRight - new Vector3(0, 4, 0)) is ObsidianBlock;
        }
    }
}
