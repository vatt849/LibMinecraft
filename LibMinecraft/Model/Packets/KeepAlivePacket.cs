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
    public class KeepAlivePacket : Packet
    {
        /// <summary>
        /// 
        /// </summary>
        int KeepAlive;
        /// <summary>
        /// 
        /// </summary>
        static Random rand = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeepAlivePacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public KeepAlivePacket()
        {
            KeepAlive = rand.Next();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeepAlivePacket"/> class.
        /// </summary>
        /// <param name="KeepAlive">The keep alive.</param>
        /// <remarks></remarks>
        public KeepAlivePacket(int KeepAlive)
        {
            this.KeepAlive = KeepAlive;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.KeepAlive; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 5; }
        }

        /// <summary>
        /// 
        /// </summary>
        static byte[] _payload = new byte[] { (byte)PacketID.KeepAlive, 0, 0, 0, 0 };
        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                Array.Copy(MakeInt(KeepAlive), 0, _payload, 1, 4);
                return _payload;
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.KeepAlive = ReadInt(Client.TcpClient.GetStream());
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
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            //if (this.KeepAlive != Server.keepAlive.KeepAlive)
            //    Server.KickPlayer(Client.PlayerEntity.Name, "Network timeout"); // TODO: Kick Packet
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException(); // TODO: When MultiplayerClient is changed to a Queue system, handle all of these
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
        public override void WritePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
