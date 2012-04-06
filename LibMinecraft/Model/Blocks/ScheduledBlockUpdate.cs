using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model
{
    /// <summary>
    /// This class is used when a block update has been scheduled
    /// </summary>
    /// <remarks></remarks>
    public class ScheduledBlockUpdate :IScheduledUpdate
    {
        /// <summary>
        /// The target world
        /// </summary>
        public World NewWorld;

        /// <summary>
        /// The target position
        /// </summary>
        public Vector3 NewPosition;
        /// <summary>
        /// Target locations block
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <remarks></remarks>
        
        public ScheduledBlockUpdate(World world, Vector3 position)
        {
            NewWorld = world;
            NewPosition = position;
        }

        /// <summary>
        /// This update uses the old Block system's scheduled updates. And is only compatable with the old system.
        /// </summary>
        /// <remarks></remarks>
        public void Update()
        {
            //Gets the target instance of the block
            Block b = NewWorld.GetBlock(NewPosition);
            //Gathers the metaData from the block
            byte metaData = b.Metadata;

            //Run the scheduled update
            b.ScheduledUpdate(NewWorld,NewPosition);

            //If the block's now current metaData is not equal to its old metaData, update the changes
            if(metaData != NewWorld.GetBlock(NewPosition).Metadata)
                NewWorld.SetBlock(NewPosition, b);
        }

        /// <summary>
        /// The amount of ticks remaining before this is called
        /// </summary>
        /// <value>The ticks remaining.</value>
        /// <remarks></remarks>
        public long TicksRemaining { get; set; }
    }
}
