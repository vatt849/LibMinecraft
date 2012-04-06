using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    public class EntityHeadLookPacket : Packet
    {
        public int EntityID { get; set; }
        public Vector3 HeadRotation { get; set; }

        public EntityHeadLookPacket()
        {

        }

        public EntityHeadLookPacket(int EntityID, Vector3 HeadRotation)
        {
            this.EntityID = EntityID;
            this.HeadRotation = HeadRotation;
        }

        public override PacketID PacketID
        {
            get { return PacketID.EntityHeadLook; }
        }

        public override int Length
        {
            get { return 6; }
        }

        public override byte[] Payload
        {
            get 
            {
                return new byte[] { (byte)PacketID }
                    .Concat(MakeInt(EntityID))
                    .Concat(new byte[]
                    {
                        MakePackedByte((float)HeadRotation.X),
                    }).ToArray();
            }
        }

        public override void ReadPacket(RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        public override void WritePacket(RemoteClient Client)
        {
            Client.TcpClient.GetStream().Write(Payload, 0, Length);
        }

        public override void WritePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
