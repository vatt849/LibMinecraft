using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// This handles the packet information for when the player looks in a different direction
    /// </summary>
    /// <remarks></remarks>
    public class PlayerLookPacket : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLookPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PlayerLookPacket()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLookPacket"/> class.
        /// </summary>
        /// <param name="Player">The player.</param>
        /// <remarks></remarks>
        public PlayerLookPacket(PlayerEntity Player)
        {
            this.Yaw = (float)Player.Rotation.X;
            this.Pitch = (float)Player.Rotation.Y;
            this.OnGround = Player.OnGround;
        }

        /// <summary>
        /// Gets or sets the yaw.
        /// </summary>
        /// <value>The yaw.</value>
        /// <remarks></remarks>
        public float Yaw { get; set; }
        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        /// <value>The pitch.</value>
        /// <remarks></remarks>
        public float Pitch { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [on ground].
        /// </summary>
        /// <value><c>true</c> if [on ground]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool OnGround { get; set; }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.PlayerLook; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 10; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] 
                {
                    (byte)PacketID
                }.Concat(MakeFloat(Yaw))
                .Concat(MakeFloat(Pitch))
                .Concat(MakeBoolean(OnGround)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.Yaw = ReadFloat(Client.TcpClient.GetStream());
            this.Pitch = ReadFloat(Client.TcpClient.GetStream());
            this.OnGround = ReadBoolean(Client.TcpClient.GetStream());
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(RemoteClient Client)
        {
            throw new InvalidOperationException();
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
            Client.PlayerEntity.Rotation.X = this.Yaw;
            Client.PlayerEntity.Rotation.Y = this.Pitch;
            Client.PlayerEntity.OnGround = this.OnGround;

            foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
            {
                c.PacketQueue.Enqueue(new EntityLookPacket(Client.PlayerEntity.ID, Client.PlayerEntity.Rotation));
                Client.PlayerEntity.HeadRotation = Client.PlayerEntity.Rotation.Clone();
            }
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }
    }
}
