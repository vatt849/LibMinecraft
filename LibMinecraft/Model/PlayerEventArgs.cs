using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;
using LibMinecraft.Server;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a player joining or leaving the server.
    /// </summary>
    /// <remarks></remarks>
    public class PlayerEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        /// <value>The player.</value>
        /// <remarks></remarks>
        public RemoteClient Player { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEventArgs"/> class.
        /// </summary>
        /// <param name="Player">The player.</param>
        /// <remarks></remarks>
        public PlayerEventArgs(RemoteClient Player)
        {
            this.Player = Player;
        }
    }
}
