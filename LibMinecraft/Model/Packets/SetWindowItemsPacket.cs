using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Packets
{
    public class SetWindowItemsPacket : Packet
    {
        public override PacketID PacketID
        {
            get { return PacketID.SetWindowItems; }
        }

        public override int Length
        {
            get { throw new NotImplementedException(); }
        }

        public override byte[] Payload
        {
            get { throw new NotImplementedException(); }
        }

        public override void ReadPacket(Server.RemoteClient Client)
        {
            throw new NotImplementedException();
        }

        public override void ReadPacket(Client.MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        public override void WritePacket(Server.RemoteClient Client)
        {
            throw new NotImplementedException();
        }

        public override void WritePacket(Client.MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        public override void HandlePacket(Server.MultiplayerServer Server, Server.RemoteClient Client)
        {
            throw new NotImplementedException();
        }

        public override void HandlePacket(Client.MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
