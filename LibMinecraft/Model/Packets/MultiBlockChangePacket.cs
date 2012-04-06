using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    public class MultiBlockChangePacket : Packet
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }
        public Dictionary<Vector3, Block> UpdatedBlocks { get; set; }

        public override PacketID PacketID
        {
            get { return PacketID.MultiBlockChange; }
        }

        public override int Length
        {
            get { return -1; }
        }

        public override byte[] Payload
        {
            get 
            {
                throw new NotImplementedException();
            }
        }

        public override void ReadPacket(RemoteClient Client)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
