using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Classic.Server;
using LibMinecraft.Classic.Client;
namespace LibMinecraft.Classic.Model.Packets
{
    /// <summary>
    /// Packet sent to server/client when sending/recieving packet.
    /// </summary>
    /// <remarks></remarks>
    public class MessagePacket : Packet
    {
        /// <summary>
        /// Gets or sets the player ID.
        /// </summary>
        /// <value>The player ID.</value>
        /// <remarks></remarks>
        public byte PlayerID { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        /// <remarks></remarks>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public MessagePacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePacket"/> class.
        /// </summary>
        /// <param name="PlayerID">The player ID.</param>
        /// <param name="Message">The message.</param>
        /// <remarks></remarks>
        public MessagePacket(byte PlayerID, string Message)
        {
            this.PlayerID = PlayerID;
            this.Message = Message;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.Message; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 66; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID, PlayerID }
                    .Concat(MakeString(Message)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            Client.TcpClient.GetStream().ReadByte(); //Unused
            Message = ReadString(Client.TcpClient.GetStream());
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
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(ClassicServer Server, RemoteClient Client)
        {
            //Send message to all clients. Create a method in ClassicServer to do this, which handles length & colors.
            Server.EnqueueToAllClients(new MessagePacket(0, "<" + Client.UserName + ">: " + Message));
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
