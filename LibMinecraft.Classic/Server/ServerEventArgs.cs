using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Classic.Server
{
    /// <summary>
    /// Called when the player connection status changes.
    /// </summary>
    /// <remarks></remarks>
    public class PlayerConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the state of the connection.
        /// </summary>
        /// <value>The state of the connection.</value>
        /// <remarks></remarks>
        public ConnectionState ConnectionState { get; set; }
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        /// <remarks></remarks>
        public RemoteClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="ConnectionState">State of the connection.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public PlayerConnectionEventArgs(ConnectionState ConnectionState, RemoteClient Client)
        {
            this.ConnectionState = ConnectionState;
            this.Client = Client;
        }
    }

    /// <summary>
    /// The connection state
    /// </summary>
    /// <remarks></remarks>
    public enum ConnectionState
    {
        /// <summary>
        /// Client is connected
        /// </summary>
        Connected,
        /// <summary>
        /// Client is connected.
        /// </summary>
        Disconnected,
    }
}
