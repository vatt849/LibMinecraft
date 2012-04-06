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
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class EntityActionPacket : Packet
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        /// <value>The entity ID.</value>
        /// <remarks></remarks>
        public int EntityID { get; set; }
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        /// <remarks></remarks>
        public EntityAction Action { get; set; }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.EntityAction; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 6; }
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
                    .Concat(MakeInt(EntityID))
                    .Concat(new byte[] { (byte)Action }).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.EntityID = ReadInt(Client.TcpClient.GetStream());
            this.Action = (EntityAction)Client.TcpClient.GetStream().ReadByte();
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(RemoteClient Client)
        {
            throw new InvalidOperationException();
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
            if (EntityID != Client.PlayerEntity.ID)
            {
                Server.KickPlayer(Client.PlayerEntity.Name, "Hacking!");
                return;
            }

            if (Action == EntityAction.Crouch || Action == EntityAction.UnCrouch)
            {
                Animation animation = Action == EntityAction.Crouch ? Animation.Crouch : Animation.UnCrouch;
                foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
                {
                    c.PacketQueue.Enqueue(new AnimationPacket(Client.PlayerEntity.ID, animation));
                }
            }
            if (Action == EntityAction.LeaveBed)
            {
                //Unoccupy the metadata of the block
                Block bed = Server.GetWorld(Client.PlayerEntity).GetBlock(Client.PlayerEntity.OccupiedBed); //Get the block
                bed.Metadata = (byte)(bed.Metadata & ~0x4); // Remove flag
                Server.GetWorld(Client.PlayerEntity).SetBlock(Client.PlayerEntity.OccupiedBed, bed); //Set block

                Client.PlayerEntity.OccupiedBed = null; //Nullify the clients bed position

                Server.EnqueueToAllClients(new AnimationPacket(Client.PlayerEntity.ID, Animation.LeaveBed));
            }

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
