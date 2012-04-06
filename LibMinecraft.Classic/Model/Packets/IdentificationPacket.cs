using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Classic.Server;
using LibMinecraft.Classic.Client;
using System.Net.Sockets;
using System.Threading;
using System.Web;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace LibMinecraft.Classic.Model.Packets
{
    /// <summary>
    /// Packet sent to server when logging in.
    /// </summary>
    /// <remarks></remarks>
    public class IdentificationPacket : Packet
    {
        /// <summary>
        /// Gets or sets the protocol version.
        /// </summary>
        /// <value>The protocol version.</value>
        /// <remarks></remarks>
        public byte ProtocolVersion { get; set; }
        /// <summary>
        /// Gets or sets the name of the user name.
        /// </summary>
        /// <value>The username or server name..</value>
        /// <remarks></remarks>
        public string UserNameOrServerName { get; set; }
        /// <summary>
        /// Gets or sets the verification key or MOTD.
        /// </summary>
        /// <value>The verification key or MOTD.</value>
        /// <remarks></remarks>
        public string VerificationKeyOrMOTD { get; set; }
        /// <summary>
        /// Gets or sets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        /// <remarks></remarks>
        public UserType UserType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public IdentificationPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationPacket"/> class.
        /// </summary>
        /// <param name="UserNameOrServerName">Name of the user name or server.</param>
        /// <param name="VerificationKeyOrMOTD">The verification key or MOTD.</param>
        /// <param name="UserType">Type of the user.</param>
        /// <remarks></remarks>
        public IdentificationPacket(string UserNameOrServerName, string VerificationKeyOrMOTD, UserType UserType)
        {
            this.ProtocolVersion = 7;
            this.UserNameOrServerName = UserNameOrServerName;
            this.VerificationKeyOrMOTD = VerificationKeyOrMOTD;
            this.UserType = UserType;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.Identification; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 131; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID, ProtocolVersion }
                    .Concat(MakeString(UserNameOrServerName))
                    .Concat(MakeString(VerificationKeyOrMOTD))
                    .Concat(new byte[] { (byte)UserType }).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.ProtocolVersion = (byte)Client.TcpClient.GetStream().ReadByte();
            this.UserNameOrServerName = ReadString(Client.TcpClient.GetStream());
            this.VerificationKeyOrMOTD = ReadString(Client.TcpClient.GetStream());
            Client.TcpClient.GetStream().ReadByte();
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(ClassicClient Client)
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
        public override void WritePacket(ClassicClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(ClassicServer Server, RemoteClient Client)
        {
            Client.LoggedIn = true;
            Client.UserName = UserNameOrServerName;
            Client.World = Server.MainLevel;
            Client.PacketQueue.Enqueue(new IdentificationPacket(Server.Server.Name, Server.Server.MotD, Client.UserType));
            Client.PlayerID = (byte)(Server.Clients.Count() - 1);
            Client.PacketQueue.Enqueue(new WorldInitializePacket());

            MemoryStream ms = new MemoryStream();
            GZipStream s = new GZipStream(ms, CompressionMode.Compress, true);
            s.Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(World.Find(Client.World).Data.Length)), 0, sizeof(int));
            s.Write(World.Find(Client.World).Data, 0, World.Find(Client.World).Data.Length);
            s.Close();
            byte[] data = ms.GetBuffer();
            ms.Close();

            double numChunks = data.Length / 1024;
            double chunksSent = 0;
            for (int i = 0; i < data.Length; i += 1024)
            {
                byte[] chunkData = new byte[1024];

                short chunkLength = 1024;
                if (data.Length - i < chunkLength)
                    chunkLength = (short)(data.Length - i);

                Array.Copy(data, i, chunkData, 0, chunkLength);

                Client.PacketQueue.Enqueue(new WorldDataChunkPacket(chunkLength, chunkData, (byte)((chunksSent / numChunks) * 100)));
                chunksSent++;
            }

            Client.PacketQueue.Enqueue(new WorldFinalizePacket(World.Find(Client.World).Width, World.Find(Client.World).Height, World.Find(Client.World).Depth));
            Client.Position = World.Find(Client.World).Spawn.Clone();
            Client.Yaw = 0; Client.Pitch = 0;
            unchecked { Client.PacketQueue.Enqueue(new PositionAndOrientationPacket((byte)-1, Client.Position, Client.Yaw, Client.Pitch)); }
            Server.LoadAllClientsToClient(Client);
            Server.EnqueueToAllClientsInWorld(new SpawnPlayerPacket(Client.PlayerID, Client.UserName, Client.Position, Client.Yaw, Client.Pitch), Client.World, Client);
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(ClassicClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
