using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    public class SetWindowItemsPacket : Packet
    {
        public SetWindowItemsPacket()
        {
        }

        public SetWindowItemsPacket(byte WindowID, Slot[] Slots)
        {
            this.WindowID = WindowID;
            this.Slots = Slots;
        }

        public byte WindowID { get; set; }
        public Slot[] Slots { get; set; }

        public override PacketID PacketID
        {
            get { return PacketID.SetWindowItems; }
        }

        public override int Length
        {
            get { return -1; }
        }

        public override byte[] Payload
        {
            get 
            {
                byte[] b = new byte[] { (byte)PacketID, WindowID }.Concat(MakeShort((short)Slots.Length)).ToArray();
                foreach (Slot s in Slots)
                    b = b.Concat(s.GetData()).ToArray();
                return b;
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
            Client.TcpClient.GetStream().Write(Payload, 0, Payload.Length);
        }

        public override void WritePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
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
