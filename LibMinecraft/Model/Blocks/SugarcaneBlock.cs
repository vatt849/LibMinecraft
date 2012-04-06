using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Sugarcane block (ID = 83)
    /// </summary>
    /// <remarks></remarks>
    public class SugarcaneBlock : Block
    {
        /// <summary>
        /// The maximum height that a Sugarcane block can grow to (Default = 3)
        /// </summary>
        public static int MaxGrowth = 3;

        /// <summary>
        /// The Block ID for this block (83)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 83; }
        }

        public override bool RequiresSupport
        {
            get
            {
                return true;
            }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Plant; }
        }

        public override void BlockUpdate(World world, Vector3 position)
        {
            ScheduleUpdate(MultiplayerServer.Random.Next(500, 2000), position, world);
        }

        public override void BlockDestroyed(World world, Vector3 position, Entity destroyedBy)
        {
            Vector3 location = position.Clone();
            location.Y++;
            while (location.Y < 128 && world.GetBlock(location) is SugarcaneBlock)
            {
                world.SetBlock(location, new AirBlock());
                location.Y++;
            }
        }

        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            ScheduleUpdate(MultiplayerServer.Random.Next(500, 2000), position, world);
            return true;
        }

        public override void ScheduledUpdate(World world, Vector3 position)
        {
            if (world.GetBlock(position + Vector3.Up) is AirBlock)
            {
                if (!(world.GetBlock(position - new Vector3(0, MaxGrowth - 1, 0)) is SugarcaneBlock))
                {
                    SugarcaneBlock b = new SugarcaneBlock();
                    world.SetBlock(position + Vector3.Up, b);
                    b.ScheduleUpdate(20, position + Vector3.Up, world);
                }
            }
        }
    }
}
