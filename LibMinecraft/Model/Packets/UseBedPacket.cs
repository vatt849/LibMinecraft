using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// The UseBed Packet.
    /// This packet is sent to all clients when a player <b>enters</b> a bed. If there
    /// is only one player on the server, this packet is not sent out.
    /// </summary>
    public class UseBedPacket : Packet
    {


        public UseBedPacket(Vector3 location, int entityID) : base()
        {
            Unknown = 0;
            EntityID = entityID;
            Location = location;
        }
        /// <summary>
        /// The UseBed packet returns PacketID.UseBed;
        /// </summary>
        public override PacketID PacketID
        {
            get { return PacketID.UseBed; }
        }

        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        /// <remarks></remarks>
        public int EntityID { get; set; }
          
        /// <summary>
        /// Gets or Sets the in bed property.
        /// </summary>
        public byte Unknown { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        /// <remarks></remarks>
        public Vector3 Location { get; set; }

        /// <summary>
        /// Gets the length of the packet/
        /// </summary>
        public override int Length
        {
            get { return 15; }
        }

        public override byte[] Payload
        {
            get
            {
                return new byte[] { (byte)PacketID }
                    .Concat(MakeInt(EntityID))
                    .Concat(new byte[] { (byte)Unknown })
                    .Concat(MakeInt((int)Location.X))
                    .Concat(new byte[] { (byte)(Location.Y+1) })
                    .Concat(MakeInt((int)Location.Z)).ToArray();
            }
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
            Client.TcpClient.GetStream().Write(Payload, 0, Length);
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
