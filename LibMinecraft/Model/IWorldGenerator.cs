using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents an object used to generate
    /// chunks for a world.
    /// </summary>
    public interface IWorldGenerator
    {
        /// <summary>
        /// The name of the world generator.
        /// </summary>
        /// <remarks>This is sent to the client in Login packets.
        /// As of 1.1, the valid values are "DEFAULT" and "FLAT"</remarks>
        string Name { get; }

        int Version { get; }

        /// <summary>
        /// Generates a chunk.
        /// </summary>
        /// <param name="Coordinates">The coordinates to generate at.</param>
        /// <param name="World">The world to generate in.</param>
        /// <param name="Dimension">The dimension that should be generated.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        MapColumn GenerateColumn(Vector3 Coordinates, Region Region, Dimension Dimension, long Seed);
    }
}
