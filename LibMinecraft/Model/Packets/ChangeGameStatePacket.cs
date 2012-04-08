using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class ChangeGameStatePacket : Packet
    {
        /// <summary>
        /// Gets or sets the state of the game.
        /// </summary>
        /// <value>The state of the game.</value>
        /// <remarks></remarks>
        public NewOrInvalidState GameState { get; set; }
        /// <summary>
        /// Only used when GameState == NewOrInvalidState.ChangeGameMode
        /// </summary>
        /// <value><c>true</c> if creative; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Creative { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeGameStatePacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public ChangeGameStatePacket()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeGameStatePacket"/> class.
        /// </summary>
        /// <param name="GameState">State of the game.</param>
        /// <remarks></remarks>
        public ChangeGameStatePacket(NewOrInvalidState GameState)
        {
            this.GameState = GameState;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeGameStatePacket"/> class.
        /// </summary>
        /// <param name="GameState">State of the game.</param>
        /// <param name="Creative">if set to <c>true</c> [creative].</param>
        /// <remarks></remarks>
        public ChangeGameStatePacket(NewOrInvalidState GameState, bool Creative)
        {
            this.GameState = GameState;
            this.Creative = Creative;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.ChangeGameState; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 3; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID, (byte)GameState, (byte)(Creative ? 1 : 0) };
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(RemoteClient Client)
        {
            Client.TcpClient.GetStream().Write(Payload, 0, Length);
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
