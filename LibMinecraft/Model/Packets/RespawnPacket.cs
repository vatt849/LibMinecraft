using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// The packet for Respawning
    /// </summary>
    /// <remarks></remarks>
    public class RespawnPacket : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RespawnPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public RespawnPacket()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RespawnPacket"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="world">The world.</param>
        /// <remarks></remarks>
        public RespawnPacket(RemoteClient client, World world)
        {
            this.Dimension = client.PlayerEntity.Dimension;
            this.Difficulty = world.Level.Difficulty;
            this.GameMode = client.PlayerEntity.GameMode;
            this.WorldHeight = World.Height;
            this.LevelType = world.Level.WorldGenerator.Name;
        }

        /// <summary>
        /// Gets or sets the dimension.
        /// </summary>
        /// <value>The dimension.</value>
        /// <remarks></remarks>
        public Dimension Dimension { get; set; }
        /// <summary>
        /// Gets or sets the difficulty.
        /// </summary>
        /// <value>The difficulty.</value>
        /// <remarks></remarks>
        public Difficulty Difficulty { get; set; }
        /// <summary>
        /// Gets or sets the game mode.
        /// </summary>
        /// <value>The game mode.</value>
        /// <remarks></remarks>
        public GameMode GameMode { get; set; }
        /// <summary>
        /// Gets or sets the height of the world.
        /// </summary>
        /// <value>The height of the world.</value>
        /// <remarks></remarks>
        public short WorldHeight { get; set; }
        /// <summary>
        /// Gets or sets the type of the level.
        /// </summary>
        /// <value>The type of the level.</value>
        /// <remarks></remarks>
        public string LevelType { get; set; }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.Respawn; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return -1; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID }
                .Concat(MakeInt((int)Dimension))
                .Concat(new byte[] { (byte)GameMode })
                .Concat(MakeShort(WorldHeight))
                .Concat(MakeString(LevelType)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.Dimension = (Dimension)Client.TcpClient.GetStream().ReadByte();
            this.Difficulty = (Difficulty)Client.TcpClient.GetStream().ReadByte();
            this.GameMode = (GameMode)Client.TcpClient.GetStream().ReadByte();
            this.WorldHeight = ReadShort(Client.TcpClient.GetStream());
            this.LevelType = ReadString(Client.TcpClient.GetStream());
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
            Client.TcpClient.GetStream().Write(Payload, 0, Payload.Length);
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            Client.PlayerEntity.Location = Server.Levels[Client.PlayerEntity.LevelIndex].Spawn;
            Client.PacketQueue.Enqueue(new RespawnPacket(Client, Server.GetWorld(Client)));
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
