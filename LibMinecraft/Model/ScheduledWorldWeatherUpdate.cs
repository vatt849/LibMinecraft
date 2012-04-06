using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Update for weather
    /// </summary>
    /// <remarks></remarks>
    public class ScheduledWorldWeatherUpdate : IScheduledUpdate
    {
        /// <summary>
        /// 
        /// </summary>
        private World NewWorld;
        /// <summary>
        /// 
        /// </summary>
        bool Weather;

        /// <summary>
        /// The function which is called when the appropriate amount of ticks have been passed.
        /// </summary>
        /// <remarks></remarks>
        public void Update()
        {
            // TODO
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledWorldWeatherUpdate"/> class.
        /// </summary>
        /// <param name="NewWorld">The new world.</param>
        /// <param name="Weather">if set to <c>true</c> [weather].</param>
        /// <remarks></remarks>
        public ScheduledWorldWeatherUpdate(World NewWorld, bool Weather){
            this.NewWorld = NewWorld;
            this.Weather = Weather;
        }

        /// <summary>
        /// The ticks remaining before this update is called.
        /// </summary>
        /// <value>The ticks remaining.</value>
        /// <remarks></remarks>
        public long TicksRemaining
        { get; set; }
    }
}
