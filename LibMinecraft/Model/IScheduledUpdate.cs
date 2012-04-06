using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model
{
    /// <summary>
    /// The interface for all scheduled updates.
    /// </summary>
    /// <remarks></remarks>
    public interface IScheduledUpdate
    {
        /// <summary>
        /// The function which is called when the appropriate ammount of ticks have been passed.
        /// </summary>
        void Update();

        /// <summary>
        /// The ticks remaining before this update is called.
        /// </summary>
        long TicksRemaining { get; set; }
    }
}
