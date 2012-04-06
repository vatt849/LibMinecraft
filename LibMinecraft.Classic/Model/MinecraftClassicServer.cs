using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// Minecraft Classic Server Model
    /// </summary>
    /// <remarks></remarks>
    public class MinecraftClassicServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinecraftClassicServer"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MinecraftClassicServer()
        {
            MotD = "Welcome to LibMinecraft!";
            Name = "A LibMinecraft Server";
            MaxPlayers = 25;
            Port = 25565;
            Private = false;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        /// <remarks></remarks>
        public int Port { get; set; }
        /// <summary>
        /// Gets or sets the max players.
        /// </summary>
        /// <value>The max players.</value>
        /// <remarks></remarks>
        public int MaxPlayers { get; set; }
        /// <summary>
        /// Gets or sets the name of the server.
        /// </summary>
        /// <value>The name.</value>
        /// <remarks></remarks>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MinecraftClassicServer"/> is private (in the server list).
        /// </summary>
        /// <value><c>true</c> if private; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Private { get; set; }
        /// <summary>
        /// Gets or sets the Message of the Day.
        /// </summary>
        /// <value>The Message of the Day.</value>
        /// <remarks></remarks>
        public string MotD { get; set; }
    }
}
