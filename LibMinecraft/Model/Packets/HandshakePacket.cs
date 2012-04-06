using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;
using LibMinecraft.Model.Entities;
using System.Security.Cryptography;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class HandshakePacket : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakePacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public HandshakePacket()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakePacket"/> class.
        /// </summary>
        /// <param name="UsernameOrHash">The username or hash.</param>
        /// <remarks></remarks>
        public HandshakePacket(string UsernameOrHash)
        {
            this.UsernameOrHash = UsernameOrHash;
        }

        /// <summary>
        /// If the hash is '-', then online-mode
        /// is disabled.
        /// </summary>
        /// <value>The username or hash.</value>
        /// <remarks></remarks>
        public string UsernameOrHash { get; set; }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.Handshake; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get
            {
                if (Payload == null)
                    return -1;
                else
                    return Payload.Length;
            }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID }.Concat(MakeString(UsernameOrHash)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.UsernameOrHash = ReadString(Client.TcpClient.GetStream());
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
        /// <param name="server">The server.</param>
        /// <param name="client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer server, RemoteClient client)
        {
            // TODO: Ban list
            client.PlayerEntity = new PlayerEntity(); // TODO: Load existing data, or create new data
            string[] parts = this.UsernameOrHash.Split(';');
            client.PlayerEntity.Name = parts[0];
            client.Hostname = parts[1];

            // Create response
            if (server.CurrentServer.OnlineMode)
            {
                MD5 md5 = MD5.Create();
                client.OnlineModeHash = new Random().Next().ToString(); // TODO
                client.PacketQueue.Enqueue(new HandshakePacket(client.OnlineModeHash));
            }
            else
            {
                client.OnlineModeHash = "-";
                client.PacketQueue.Enqueue(new HandshakePacket("-"));
            }
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(Client.MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
