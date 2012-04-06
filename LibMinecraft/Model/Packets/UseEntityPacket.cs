using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// The client sends this packet when clicking on entities.
    /// </summary>
    public class UseEntityPacket : Packet
    {
        /// <summary>
        /// This is the entity ID of the clicking user
        /// </summary>
        public int UserEntityID { get; set; }
        /// <summary>
        /// This is the entity ID that they are clicking on
        /// </summary>
        public int TargetEntityID { get; set; }
        /// <summary>
        /// True if the user is left-clicking the entity
        /// </summary>
        public bool LeftClick { get; set; }

        /// <summary>
        /// The packet ID for this packet, in this case, UseEntity
        /// </summary>
        public override PacketID PacketID
        {
            get { return PacketID.UseEntity; }
        }

        /// <summary>
        /// Gets the length of this packet (10)
        /// </summary>
        public override int Length
        {
            get { return 10; }
        }

        public override byte[] Payload
        {
            get 
            {
                return new byte[] { (byte)PacketID }
                    .Concat(MakeInt(UserEntityID))
                    .Concat(MakeInt(TargetEntityID))
                    .Concat(MakeBoolean(LeftClick)).ToArray();
            }
        }

        public override void ReadPacket(RemoteClient Client)
        {
            UserEntityID = ReadInt(Client.TcpClient.GetStream());
            TargetEntityID = ReadInt(Client.TcpClient.GetStream());
            LeftClick = ReadBoolean(Client.TcpClient.GetStream());
        }

        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        public override void WritePacket(RemoteClient Client)
        {
            throw new NotImplementedException();
        }

        public override void WritePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            // TODO
        }

        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
