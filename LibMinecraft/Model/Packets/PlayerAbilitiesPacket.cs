using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    public class PlayerAbilitiesPacket : Packet
    {
        public bool IsInvulnerable { get; set; }
        public bool IsFlying { get; set; }
        public bool MayFly { get; set; }
        public bool MayInstantMine { get; set; }

        public override PacketID PacketID
        {
            get { throw new NotImplementedException(); }
        }

        public override int Length
        {
            get { return 5; }
        }

        public override byte[] Payload
        {
            get { return new byte[] { (byte)PacketID }
                .Concat(MakeBoolean(IsInvulnerable))
                .Concat(MakeBoolean(IsFlying))
                .Concat(MakeBoolean(MayFly))
                .Concat(MakeBoolean(MayInstantMine)).ToArray(); }
        }

        public override void ReadPacket(RemoteClient Client)
        {
            IsInvulnerable = ReadBoolean(Client.TcpClient.GetStream());
            IsFlying = ReadBoolean(Client.TcpClient.GetStream());
            MayFly = ReadBoolean(Client.TcpClient.GetStream());
            MayInstantMine = ReadBoolean(Client.TcpClient.GetStream());
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
