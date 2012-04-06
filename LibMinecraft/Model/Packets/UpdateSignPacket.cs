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
    /// Represents a packet containing information about a sign
    /// </summary>
    /// <remarks></remarks>
    public class UpdateSignPacket : Packet
    {
        /// <summary>
        /// Gets or sets the sign location.
        /// </summary>
        /// <value>The sign location.</value>
        /// <remarks></remarks>
        public Vector3 SignLocation { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        /// <remarks></remarks>
        public string[] Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public UpdateSignPacket()
        {
            Text = new string[] { "", "", "", "" };
            SignLocation = Vector3.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSignPacket"/> class.
        /// </summary>
        /// <param name="SignLocation">The sign location.</param>
        /// <param name="Text">The text.</param>
        /// <remarks></remarks>
        public UpdateSignPacket(Vector3 SignLocation, string[] Text)
        {
            this.SignLocation = SignLocation;
            this.Text = Text;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.UpdateSign; }
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
                    .Concat(MakeInt((int)SignLocation.X))
                    .Concat(MakeShort((short)SignLocation.Y))
                    .Concat(MakeInt((int)SignLocation.Z))
                    .Concat(MakeString(Text[0]))
                    .Concat(MakeString(Text[1]))
                    .Concat(MakeString(Text[2]))
                    .Concat(MakeString(Text[3])).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            SignLocation.X = ReadInt(Client.TcpClient.GetStream());
            SignLocation.Y = ReadShort(Client.TcpClient.GetStream());
            SignLocation.Z = ReadInt(Client.TcpClient.GetStream());
            Text[0] = ReadString(Client.TcpClient.GetStream());
            Text[1] = ReadString(Client.TcpClient.GetStream());
            Text[2] = ReadString(Client.TcpClient.GetStream());
            Text[3] = ReadString(Client.TcpClient.GetStream());
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

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            Block b = Server.GetWorld(Client).GetBlock(this.SignLocation);
            if (!(b is SignPostBlock) && !(b is WallSignBlock))
            {
                Server.KickPlayer(Client.PlayerEntity.Name, "Sign hacking!");
                return;
            }
            if (b is SignPostBlock)
                (b as SignPostBlock).Data.Text = this.Text;
            else
                (b as WallSignBlock).Data.Text = this.Text;
            Server.Levels[Client.PlayerEntity.LevelIndex].SetBlock(this.SignLocation, b);
            Server.EnqueueToAllClientsExcept(this, Client.PlayerEntity);
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
