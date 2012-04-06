using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Nether Portal block (ID = 90)
    /// </summary>
    /// <remarks></remarks>
    public class NetherPortalBlock : Block
    {
        /// <summary>
        /// If set to false, portal blocks will not attempt to
        /// confirm they are within a frame when updated.
        /// </summary>
        public static bool EnableUpdates = true;

        /// <summary>
        /// The Block ID for this block (90)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 90; }
        }
        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolid; }
        }

        public override void BlockUpdate(World world, Vector3 position)
        {
            // Check to see it is still in the proper configuration
            if (!ValidatePortal(world, position))
                world.SetBlock(position, new AirBlock());
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
                if (!(b is NetherPortalBlock))
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
                if (!(a is NetherPortalBlock) ||
                    !(b is NetherPortalBlock) ||
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
