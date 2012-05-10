using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Server;
using System.Reflection;

namespace LibMinecraft.Model
{
    /// <summary>
    /// The default world generator for LibMinecraft
    /// </summary>
    /// <remarks>This generates a world with the following rules:
    /// At Y=0 is a solid layer of bedrock
    /// At Y=15 is a solid layer of grass
    /// Between these two layers is dirt</remarks>
    public class DefaultGenerator : IWorldGenerator
    {
        /// <summary>
        /// Gets the name of this generator.
        /// </summary>
        /// <remarks>Always returns "FLAT"</remarks>
        public string Name { get { return "FLAT"; } }

        public int Version { get { return 1; } }

        public bool GenerateStructures { get { return false; } set { } }

        /// <summary>
        /// Generates a chunk.
        /// </summary>
        /// <param name="Coordinates">The coordinates to generate at.</param>
        /// <param name="World">The world to generate in.</param>
        /// <param name="Dimension">The dimension that should be generated.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public MapColumn GenerateColumn(Vector3 Coordinates, Region Region, Dimension Dimension, long Seed)
        {
            MapColumn mc = new MapColumn(Region, Coordinates);

            switch (Dimension)
            {
                case Dimension.Nether:
                    for (int x = 0; x < 16; x++)
                        for (int y = 0; y < 15; y++)
                            for (int z = 0; z < 16; z++)
                                mc.SetBlock(new Vector3(x, y, z), new NetherrackBlock());
                    break;
                default:
                    // Top layer of grass, bottom layer of bedrock
                    for (int x = 0; x < 16; x++)
                        for (int z = 0; z < 16; z++)
                        {
                            mc.SetBlock(new Vector3(x, 15, z), new GrassBlock());
                            for (int y = 1; y < 15; y++)
                                mc.SetBlock(new Vector3(x, y, z), new DirtBlock());
                            mc.SetBlock(new Vector3(x, 0, z), new BedrockBlock());
                        }
                    Vector3 location = Vector3.Abs(mc.Location.Clone()).Clamp(15);
                    mc.SetBlock(new Vector3(location.X, 16, location.Z), new GlassBlock());
                    break;
            }

            return mc;
        }
    }
}
