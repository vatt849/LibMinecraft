using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Managing class for ScheduledUpdates.
    /// </summary>
    /// <remarks></remarks>
    public static class ScheduledUpdateManager
    {
        /// <summary>
        /// Updates that are queued to be ran.
        /// </summary>
        public static List<IScheduledUpdate> Updates;

        /// <summary>
        /// Function that adds a new ScheduledUpdate from an external point.
        /// </summary>
        /// <param name="TicksRemaining">The ticks remaining.</param>
        /// <param name="newUpdate">The new update.</param>
        /// <remarks></remarks>
        public static void AddUpdate(long TicksRemaining, IScheduledUpdate newUpdate)
        {
            newUpdate.TicksRemaining = TicksRemaining;
            Updates.Add(newUpdate);
        }

        /// <summary>
        /// Ran every tick. Manages ScheduledUpdates. TODO:: Better document this file
        /// </summary>
        public static void Tick()
        {
            //Loop through ScheduledUpdates
            for (int i = 0; i < Updates.Count; i++)
            {
                //The current tick count on the currently examined scheduledUpdate
                long curTicksRemaining = Updates[i].TicksRemaining;

                //Subtract the tick count if it exists
                 Updates[i].TicksRemaining--;

                //If there are 0 ticks remaining or no ticks to begin with, instantly implement the scheduledupdate
                if (curTicksRemaining <= 0)
                {
                    Updates[i].Update();

                    //Remove the Update after updating
                    Updates.RemoveAt(i);
                    //move the iterator back one.
                    i--;
                }
            }
        }

        /// <summary>
        /// Static constructor for Updatemanager. Sets up the list.
        /// </summary>
        static ScheduledUpdateManager()
        {
            Updates = new List<IScheduledUpdate>();
        }

        internal static void RemoveBlockUpdates(Vector3 position)
        {
            for (int i = 0; i < Updates.Count; i++)
            {
                if (Updates[i] is ScheduledBlockUpdate && ((ScheduledBlockUpdate)Updates[i]).NewPosition.Equals(position))
                {
                    Updates.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
