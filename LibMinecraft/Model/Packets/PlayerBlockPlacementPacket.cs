using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Client;
using LibMinecraft.Server;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Model.Items;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class PlayerBlockPlacementPacket : Packet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerBlockPlacementPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public PlayerBlockPlacementPacket()
        {
            this.Location = new Vector3();
        }

        /// <summary>
        /// 
        /// </summary>
        private static Type[] ValidPlacementBlocks = new Type[]
        {
            typeof(AirBlock),
            typeof(LavaStillBlock),
            typeof(WaterStillBlock),
            typeof(LavaFlowingBlock),
            typeof(WaterFlowingBlock),
            typeof(FireBlock),
        };

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        /// <remarks></remarks>
        public Vector3 Location { get; set; }
        /// <summary>
        /// Gets or sets the slot.
        /// </summary>
        /// <value>The slot.</value>
        /// <remarks></remarks>
        public Slot Slot { get; set; }
        /// <summary>
        /// Gets or sets the facing.
        /// </summary>
        /// <value>The facing.</value>
        /// <remarks></remarks>
        public byte Facing { get; set; }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.PlayerBlockPlacement; }
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
                    .Concat(MakeInt((int)Location.X))
                    .Concat(new byte[] { (byte)Location.Y })
                    .Concat(MakeInt((int)Location.Z))
                    .Concat(new byte[] { Facing })
                    .Concat(Slot.GetData()).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            this.Location.X = ReadInt(Client.TcpClient.GetStream());
            this.Location.Y = Client.TcpClient.GetStream().ReadByte();
            this.Location.Z = ReadInt(Client.TcpClient.GetStream());
            this.Facing = (byte)Client.TcpClient.GetStream().ReadByte();
            this.Slot = Slot.ReadSlot(Client.TcpClient.GetStream());
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
            Block clickedBlock = new AirBlock();
            if (Location.Y < 256)
                clickedBlock = Server.GetWorld(Client).GetBlock(Location);
            if (Slot.ID != -1)
            {
                Vector3 newBlockLocation = Location.Clone();
                switch (Facing)
                {
                    case 0:
                        newBlockLocation.Y--;
                        break;
                    case 1:
                        newBlockLocation.Y++;
                        break;
                    case 2:
                        newBlockLocation.Z--;
                        break;
                    case 3:
                        newBlockLocation.Z++;
                        break;
                    case 4:
                        newBlockLocation.X--;
                        break;
                    case 5:
                        newBlockLocation.X++;
                        break;
                }

                if (Location.Y < 256)
                {
                    if (Slot.ID < 256)
                    {
                        Block newBlock = (Block)Slot.ID;
                        newBlock.Metadata = (byte)Slot.Metadata;

                        if (clickedBlock.BlockRightClicked(Server.GetWorld(Client), Location, Client.PlayerEntity))
                        {
                            if (newBlock.BlockPlaced(Server.GetWorld(Client), newBlockLocation, Location, Facing, Client.PlayerEntity))
                            {
                                Block current = Server.GetWorld(Client).GetBlock(newBlockLocation);
                                if (ValidPlacementBlocks.Contains(current.GetType()))
                                {
                                    foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
                                        c.PacketQueue.Enqueue(new AnimationPacket(Client.PlayerEntity.ID, Animation.SwingArm));

                                    Server.GetWorld(Client).SetBlock(newBlockLocation, newBlock);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (clickedBlock.BlockRightClicked(Server.GetWorld(Client), Location, Client.PlayerEntity))
                        {
                            Item item = (Item)Slot.ID;
                            item.Metadata = (byte)Slot.Metadata;
                            item.Damage = Slot.Count;
                            foreach (RemoteClient c in Server.GetClientsInWorldExcept(Server.GetWorld(Client), Client))
                                c.PacketQueue.Enqueue(new AnimationPacket(Client.PlayerEntity.ID, Animation.SwingArm));
                            item.PlayerUseItem(Server.GetWorld(Client), Client.PlayerEntity, newBlockLocation, Location, Facing);
                        }
                    }
                }
            }
            else
                clickedBlock.BlockRightClicked(Server.GetWorld(Client), Location, Client.PlayerEntity);
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
