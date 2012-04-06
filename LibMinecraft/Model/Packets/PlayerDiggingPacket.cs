using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class PlayerDiggingPacket : Packet
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        /// <remarks></remarks>
        public byte Status { get; set; }
        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X.</value>
        /// <remarks></remarks>
        public int X { get; set; }
        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y.</value>
        /// <remarks></remarks>
        public byte Y { get; set; }
        /// <summary>
        /// Gets or sets the Z.
        /// </summary>
        /// <value>The Z.</value>
        /// <remarks></remarks>
        public int Z { get; set; }
        /// <summary>
        /// Gets or sets the face.
        /// </summary>
        /// <value>The face.</value>
        /// <remarks></remarks>
        public byte Face { get; set; }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.PlayerDigging; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 12; }
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
                    (byte)PacketID,
                    Status,
                }.Concat(MakeInt(X))
                .Concat(new byte[] { Y })
                .Concat(MakeInt(Z))
                .Concat(new byte[] { Face }).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.Status = (byte)Client.TcpClient.GetStream().ReadByte();
            this.X = ReadInt(Client.TcpClient.GetStream());
            this.Y = (byte)Client.TcpClient.GetStream().ReadByte();
            this.Z = ReadInt(Client.TcpClient.GetStream());
            this.Face = (byte)Client.TcpClient.GetStream().ReadByte();
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
        /// <param name="client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient client)
        {
            // TODO: More stuff
            Block b = Server.Levels[client.PlayerEntity.LevelIndex].GetBlock(new Vector3(X, Y, Z));

            if (Status == 0x00 && client.PlayerEntity.GameMode == GameMode.Creative ||
                Server.Levels[client.PlayerEntity.LevelIndex].GetBlock(new Vector3(X, Y, Z)).InstantBreak)
                Status = 0x02;

            switch (Status)
            {
                case 0x00:
                    foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(client), client))
                        c.PacketQueue.Enqueue(new AnimationPacket(client.PlayerEntity.ID, Animation.SwingArm));
                    break;
                case 0x02:
                    Block blockDestroyed = Server.Levels[client.PlayerEntity.LevelIndex].GetBlock(new Vector3(X, Y, Z));
                    blockDestroyed.BlockDestroyed(Server.GetWorld(client), new Vector3(X, Y, Z),
                        client.PlayerEntity);
                    Server.GetWorld(client).SetBlock(new Vector3(X, Y, Z), new AirBlock());
                    foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(client), client))
                        c.PacketQueue.Enqueue(new AnimationPacket(client.PlayerEntity.ID, Animation.SwingArm));
                    Server.EnqueueToAllClientsExcept(new SoundOrParticleEffectPacket(SoundOrParticleEffect.BlockBreak,
                            new Vector3(X, Y, Z), blockDestroyed), client.PlayerEntity);
                    break;
                case 0x04:
                case 0x05:
                    break;
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
