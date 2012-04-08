using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;
using System.Net;
using System.IO;
using LibMinecraft.Model.Entities;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class LoginRequestPacket : Packet
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        private int protocolVersionOrEID;
        /// <summary>
        /// Gets or sets the protocol version or EID.
        /// </summary>
        /// <value>The protocol version or EID.</value>
        /// <remarks></remarks>
        public int ProtocolVersionOrEID
        {
            get
            {
                return protocolVersionOrEID;
            }
            set
            {
                protocolVersionOrEID = value;
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        /// <remarks></remarks>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the type of the level.
        /// </summary>
        /// <value>The type of the level.</value>
        /// <remarks></remarks>
        public string LevelType { get; set; }
        /// <summary>
        /// Gets or sets the world mode.
        /// </summary>
        /// <value>The world mode.</value>
        /// <remarks></remarks>
        public GameMode WorldMode { get; set; }
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
        /// Gets or sets the max players.
        /// </summary>
        /// <value>The max players.</value>
        /// <remarks></remarks>
        public byte MaxPlayers { get; set; }

        #endregion

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.LoginRequest; }
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
                return new byte[] { (byte)PacketID.LoginRequest, }
                    .Concat(MakeInt(protocolVersionOrEID)).ToArray()
                    .Concat(MakeString(Username)).ToArray()
                    .Concat(MakeString(LevelType)).ToArray()
                    .Concat(MakeInt((int)WorldMode)).ToArray()
                    .Concat(MakeInt((int)Dimension)).ToArray()
                    .Concat(new byte[] { (byte)Difficulty }).ToArray()
                    .Concat(new byte[] { 0 }).ToArray()
                    .Concat(new byte[] { MaxPlayers, }).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            ProtocolVersionOrEID = ReadInt(Client.TcpClient.GetStream());
            Username = ReadString(Client.TcpClient.GetStream());
            LevelType = ReadString(Client.TcpClient.GetStream());
            WorldMode = (GameMode)ReadInt(Client.TcpClient.GetStream());
            Dimension = (Dimension)ReadInt(Client.TcpClient.GetStream());
            Difficulty = (Difficulty)Client.TcpClient.GetStream().ReadByte();
            Client.TcpClient.GetStream().ReadByte();
            MaxPlayers = (byte)Client.TcpClient.GetStream().ReadByte();
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
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient client)
        {
            if (client.LoggedIn)
            {
                Server.KickPlayer(client.PlayerEntity.Name, "Invalid packet recieved!");
                return;
            }
            if (ProtocolVersionOrEID < MultiplayerServer.ProtocolVersion)
            {
                client.PacketQueue.Clear();
                client.PacketQueue.Enqueue(new DisconnectPacket("Outdated client!"));
                return;
            }
            if (ProtocolVersionOrEID > MultiplayerServer.ProtocolVersion)
            {
                client.PacketQueue.Clear();
                client.PacketQueue.Enqueue(new DisconnectPacket("Outdated server!"));
                return;
            }

            client.PlayerEntity.Name = Username;
            client.PlayerEntity.ID = MultiplayerServer.NextEntityID++;
            client.PlayerEntity.Health = 20;
            client.PlayerEntity.Food = 20;
            client.PlayerEntity.FoodSaturation = 1;
            client.PlayerEntity.LevelIndex = 0;
            client.PlayerEntity.LoadedColumns = new List<Vector3>();

            if (Server.CurrentServer.OnlineMode)
            {
                WebClient webClient = new WebClient();
                try
                {
                    StreamReader r = new StreamReader(webClient.OpenRead("http://session.minecraft.net/game/checkserver.jsp?user="
                        + Username + "&serverId=" + client.OnlineModeHash));
                    if (!r.ReadToEnd().ToUpper().Contains("YES"))
                    {
                        Server.KickPlayer(client.PlayerEntity.Name, "Failed to verify username!");
                        r.Close();
                        return;
                    }
                    r.Close();
                }
                catch
                {
                    Server.KickPlayer(client.PlayerEntity.Name, "Failed to verify username!");
                    return;
                }
            }

            this.ProtocolVersionOrEID = client.PlayerEntity.ID;
            this.WorldMode = Server.Levels[0].GameMode;
            this.LevelType = Server.Levels[0].WorldGenerator.Name;
            this.IsServerContext = false;
            this.MaxPlayers = (byte)Server.CurrentServer.MaxPlayers;
            client.PacketQueue.Enqueue(this);
            this.IsServerContext = true;
            this.Username = client.PlayerEntity.Name;

            ChunkManager.RecalculateClientColumns(client, Server, true);

            // TODO: Send inventory
            client.PacketQueue.Enqueue(new SpawnPositionPacket(Server.Levels[client.PlayerEntity.LevelIndex].Spawn));
            client.PlayerEntity.Location = Server.Levels[client.PlayerEntity.LevelIndex].Spawn;

            // Send Position
            client.PacketQueue.Enqueue(new PlayerPositionAndLookPacket(client.PlayerEntity));
            client.LoggedIn = true;
            client.PlayerEntity.GameMode = Server.Levels[client.PlayerEntity.LevelIndex].GameMode;

            // Send the new client to all logged in clients
            Server.EnqueueToAllClients(new PlayerListItemPacket(client.PlayerEntity.Name, true, 0));

            // Send all logged in clients to the new client
            foreach (RemoteClient c in Server.GetLoggedInClients())
                if (c.PlayerEntity.Name != client.PlayerEntity.Name)
                    client.PacketQueue.Enqueue(new PlayerListItemPacket(c.PlayerEntity.Name, true, 0));

            // Add entity
            Server.GetWorld(client).AddEntity(client.PlayerEntity);
            foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(client), client))
            {
                c.PacketQueue.Enqueue(new NamedEntitySpawnPacket(client.PlayerEntity.ID, client.PlayerEntity.Name,
                    client.PlayerEntity.Location, client.PlayerEntity.Rotation, client.PlayerEntity.InHand.ID));
                c.PacketQueue.Enqueue(new EntityPacket());

                client.PacketQueue.Enqueue(new NamedEntitySpawnPacket(c.PlayerEntity.ID, c.PlayerEntity.Name,
                    c.PlayerEntity.Location, c.PlayerEntity.Rotation, c.PlayerEntity.InHand.ID));
            }
            client.PacketQueue.Enqueue(new TimeUpdatePacket(Server.Levels[client.PlayerEntity.LevelIndex].Time));

            if (Server.GetLevel(client).WeatherManager.WeatherOccuring)
                client.PacketQueue.Enqueue(new NewOrInvalidStatePacket(NewOrInvalidState.BeginRain));
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException(); // TODO
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
