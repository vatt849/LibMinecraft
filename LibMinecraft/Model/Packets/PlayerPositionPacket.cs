using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// Represents a PlayerPostitionPacket (0x0B)
    /// </summary>
    public class PlayerPositionPacket : Packet
    {
        /// <summary>
        /// The location sent by the client
        /// </summary>
        public Vector3 Location { get; set; }
        /// <summary>
        /// The stance sent by the client
        /// </summary>
        public double Stance { get; set; }
        /// <summary>
        /// Whether the client is on the ground, as sent by the client
        /// </summary>
        public bool OnGround { get; set; }

        /// <summary>
        /// PacketID.PlayerPosition
        /// </summary>
        public override PacketID PacketID
        {
            get { return PacketID.PlayerPosition; }
        }

        /// <summary>
        /// 34 bytes
        /// </summary>
        public override int Length
        {
            get { return 34; }
        }

        /// <summary>
        /// Gets the payload of this packet
        /// </summary>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID }
                    .Concat(MakeDouble(Location.X))
                    .Concat(MakeDouble(Stance))
                    .Concat(MakeDouble(Location.Y))
                    .Concat(MakeDouble(Location.Z))
                    .Concat(MakeBoolean(OnGround)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet in from the cleint
        /// </summary>
        public override void ReadPacket(RemoteClient Client)
        {
            this.Location = new Vector3();
            Location.X = ReadDouble(Client.TcpClient.GetStream());
            Location.Y = ReadDouble(Client.TcpClient.GetStream());
            Stance = ReadDouble(Client.TcpClient.GetStream());
            Location.Z = ReadDouble(Client.TcpClient.GetStream());
            OnGround = ReadBoolean(Client.TcpClient.GetStream());
        }

        /// <summary>
        /// Throws InvalidOperationException
        /// </summary>
        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Throws InvalidOperationException
        /// </summary>
        public override void WritePacket(RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Throws NotImplementedException
        /// </summary>
        /// <param name="Client"></param>
        public override void WritePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the packet when sent from a client
        /// </summary>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            Client.PlayerEntity.OldLocation = Client.PlayerEntity.Location.Clone();
            Client.PlayerEntity.Location = Location;
            Client.PlayerEntity.Stance = Stance;
            Client.PlayerEntity.OnGround = OnGround;

            if (Math.Abs(Client.PlayerEntity.Location.X - Client.PlayerEntity.OldLocation.X) < 4 &&
                Math.Abs(Client.PlayerEntity.Location.Y - Client.PlayerEntity.OldLocation.Y) < 4 &&
                Math.Abs(Client.PlayerEntity.Location.Z - Client.PlayerEntity.OldLocation.Z) < 4)
            {
                foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
                {
                    c.PacketQueue.Enqueue(new EntityRelativeMovePacket(Client.PlayerEntity.ID, Client.PlayerEntity.OldLocation,
                        Client.PlayerEntity.Location));
                }
            }
            else
            {
                foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
                {
                    c.PacketQueue.Enqueue(new EntityTeleportPacket(Client.PlayerEntity.ID, Client.PlayerEntity.Location,
                        Client.PlayerEntity.Rotation));
                }
            }
        }

        /// <summary>
        /// Throws InvalidOperationException
        /// </summary>
        /// <param name="Client"></param>
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }
    }
}
