using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Flowing Lava block (ID = 10)
    /// </summary>
    /// <remarks></remarks>
    public class LavaFlowingBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (10)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 10; }
        }

        public bool CanSpread
        {
            get
            {
                return (this.Metadata & 8) == 8;
            }
            set
            {
                unchecked
                {
                    if (value)
                    {
                        this.Metadata |= 8;
                    }
                    else
                    {
                        this.Metadata &= (byte)~8;
                    }
                }
            }
        }

        public byte Level
        {
            get
            {
                return (byte)(this.Metadata & 8);
            }
            set
            {
                this.Metadata = (byte)(value | (CanSpread ? 1 : 0));
            }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.Fluid; }
        }

        public LavaFlowingBlock()
        {
        }

        public LavaFlowingBlock(byte Level, bool CanSpread = true)
        {
            this.Level = Level;
            this.CanSpread = CanSpread;
            this.Metadata = Level;
        }

        public override void BlockUpdate(World world, Vector3 position)
        {
            ScheduleUpdate(25, position, world);
        }

        public void ScheduledUpdate_(World world, Vector3 position)
        {
            // Rules:
            // 1. 
        }

        public override void ScheduledUpdate(World world, Vector3 position)
        {
            bool canSpread = !(world.GetBlock(position + Vector3.Down) is AirBlock);

            // Check inward flow
            Block above = world.GetBlock(position + Vector3.Up);
            Block[] neighbors = new Block[]
            {
                world.GetBlock(position + Vector3.Forward),
                world.GetBlock(position + Vector3.Backward),
                world.GetBlock(position + Vector3.Left),
                world.GetBlock(position + Vector3.Right),
            };
            if (this.Metadata != 0)
            {
                if (above is LavaFlowingBlock || above is LavaStillBlock)
                    this.Metadata = 1;
                else
                {
                    byte minLevel = 4;
                    int sourceCount = 0;
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (neighbors[i] is LavaFlowingBlock ||
                            neighbors[i] is LavaStillBlock)
                        {
                            if (neighbors[i].Metadata < minLevel)
                                minLevel = neighbors[i].Metadata;
                            if (neighbors[i].Metadata == 0)
                                sourceCount++;
                        }
                    }
                    if (sourceCount >= 2)
                    {
                        this.Metadata = 0;
                        return;
                    }
                    if (minLevel > 0)
                    {
                        this.Metadata = (byte)(minLevel + 1);
                        if (this.Metadata > 3)
                        {
                            this.Metadata = 3;
                            world.SetBlock(position, new AirBlock());
                            return;
                        }
                    }
                }
            }

            // Check outward flow
            if (this.Metadata < 3)
            {
                Vector3 shortestLocation = new Vector3(6, 0, 6);
                List<Vector3> additionalLocations = new List<Vector3>();
                for (int x = -5; x <= 5; x++)
                    for (int z = -5; z <= 5; z++)
                    {
                        if (Math.Abs(x) + Math.Abs(z) > 5)
                            continue;
                        if (world.GetBlock(position + new Vector3(x, -1, z)) is AirBlock)
                        {
                            if (position.DistanceTo(position + new Vector3(x, 0, z)) ==
                                position.DistanceTo(position + shortestLocation))
                                additionalLocations.Add(new Vector3(x, 0, z));
                            if (position.DistanceTo(position + new Vector3(x, 0, z)) <
                                position.DistanceTo(position + shortestLocation))
                            {
                                shortestLocation = new Vector3(x, 0, z);
                                additionalLocations.Clear();
                            }
                        }
                    }
                if (shortestLocation.Equals(new Vector3(6, 0, 6)))
                {
                    additionalLocations.Add(new Vector3(-6, 0, 6));
                    additionalLocations.Add(new Vector3(6, 0, -6));
                    additionalLocations.Add(new Vector3(-6, 0, -6));
                }
                additionalLocations.Add(shortestLocation);
                foreach (Vector3 v in additionalLocations)
                {
                    Vector3 vC = v.Clamp(1);
                    Vector3 newLocation = position + new Vector3(vC.X, 0, 0);
                    Block b = world.GetBlock(newLocation);
                    if (b is AirBlock)
                        world.SetBlock(newLocation, new LavaFlowingBlock((byte)(this.Metadata + 1)));
                    newLocation = position + new Vector3(0, 0, vC.Z);
                    b = world.GetBlock(newLocation);
                    if (b is AirBlock)
                        world.SetBlock(newLocation, new LavaFlowingBlock((byte)(this.Metadata + 1)));
                }
            }
        }
    }
}
