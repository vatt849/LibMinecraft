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
    /// Represents the position and look direction
    /// of a player
    /// </summary>
    /// <remarks></remarks>
    public class PlayerPositionAndLookPacket : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerPositionAndLookPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PlayerPositionAndLookPacket()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerPositionAndLookPacket"/> class.
        /// </summary>
        /// <param name="PlayerEntity">The player entity.</param>
        /// <remarks></remarks>
        public PlayerPositionAndLookPacket(PlayerEntity PlayerEntity)
        {
            this.Position = PlayerEntity.Location.Clone();
            this.Yaw = (float)PlayerEntity.Rotation.X;
            this.Pitch = (float)PlayerEntity.Rotation.Y;
            this.OnGround = PlayerEntity.OnGround;
            this.IsServerContext = true;
        }

        /// <summary>
        /// The player's position
        /// </summary>
        /// <value>The position.</value>
        /// <remarks></remarks>
        public Vector3 Position { get; set; }
        /// <summary>
        /// The player's stance
        /// </summary>
        /// <value>The stance.</value>
        /// <remarks></remarks>
        public double Stance { get; set; }
        /// <summary>
        /// The player's yaw value
        /// </summary>
        /// <value>The yaw.</value>
        /// <remarks></remarks>
        public float Yaw { get; set; }
        /// <summary>
        /// The player's pitch value
        /// </summary>
        /// <value>The pitch.</value>
        /// <remarks></remarks>
        public float Pitch { get; set; }
        /// <summary>
        /// Whether the player is on the ground
        /// (according to the client)
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
            get { return PacketID.PlayerPositionAndLook; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 42; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                if (this.IsServerContext)
                {
                    if (Stance - Position.Y < 0.1 ||
                        Stance - Position.Y > 1.65)
                        Stance = Position.Y + 1;
                    return new byte[]
                    {
                        (byte)PacketID,
                    }.Concat(MakeDouble(Position.X))
                    .Concat(MakeDouble(Stance))
                    .Concat(MakeDouble(Position.Y))
                    .Concat(MakeDouble(Position.Z))
                    .Concat(MakeFloat(Yaw))
                    .Concat(MakeFloat(Pitch))
                    .Concat(MakeBoolean(OnGround)).ToArray();
                }
                else
                {
                    return new byte[]
                    {
                        (byte)PacketID,
                    }.Concat(MakeDouble(Position.X))
                    .Concat(MakeDouble(Position.Y))
                    .Concat(MakeDouble(Stance))
                    .Concat(MakeDouble(Position.Z))
                    .Concat(MakeFloat(Yaw))
                    .Concat(MakeFloat(Pitch))
                    .Concat(MakeBoolean(OnGround)).ToArray();
                }
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient client)
        {
            Position = new Vector3();
            Position.X = ReadDouble(client.TcpClient.GetStream());
            Position.Y = ReadDouble(client.TcpClient.GetStream());
            Stance = ReadDouble(client.TcpClient.GetStream());
            Position.Z = ReadDouble(client.TcpClient.GetStream());
            Yaw = ReadFloat(client.TcpClient.GetStream());
            Pitch = ReadFloat(client.TcpClient.GetStream());
            OnGround = ReadBoolean(client.TcpClient.GetStream());
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
            Client.PlayerEntity.OldLocation = Client.PlayerEntity.Location.Clone();
            Client.PlayerEntity.Location = Position;
            Client.PlayerEntity.OnGround = OnGround;
            Client.PlayerEntity.Rotation.X = Yaw;
            Client.PlayerEntity.Rotation.Y = Pitch;

            if (Math.Abs(Client.PlayerEntity.Location.X - Client.PlayerEntity.OldLocation.X) < 4 &&
                Math.Abs(Client.PlayerEntity.Location.Y - Client.PlayerEntity.OldLocation.Y) < 4 &&
                Math.Abs(Client.PlayerEntity.Location.Z - Client.PlayerEntity.OldLocation.Z) < 4)
            {
                foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
                {
                    c.PacketQueue.Enqueue(new EntityLookAndRelativeMovePacket(Client.PlayerEntity.ID, Client.PlayerEntity.OldLocation,
                        Client.PlayerEntity.Location, Client.PlayerEntity.Rotation));
                    Client.PlayerEntity.HeadRotation = Client.PlayerEntity.Rotation.Clone();
                }
            }
            else
            {
                foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
                {
                    c.PacketQueue.Enqueue(new EntityTeleportPacket(Client.PlayerEntity.ID, Client.PlayerEntity.Location,
                        Client.PlayerEntity.Rotation));
                    Client.PlayerEntity.HeadRotation = Client.PlayerEntity.Rotation.Clone();
                }
            }
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
