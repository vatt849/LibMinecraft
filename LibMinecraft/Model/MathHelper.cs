using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Used to perform various mathematical operations that
    /// are relevant to Minecraft.
    /// </summary>
    /// <remarks></remarks>
    public class MathHelper
    {
        /// <summary>
        /// Gets a byte representing block direction based on the rotation
        /// of the entity that placed it, on a flat plane.
        /// </summary>
        /// <param name="p">The entity whose rotation should be used.</param>
        /// <param name="invert">If set to <c>true</c> the direction is inverted.</param>
        /// <returns></returns>
        /// <remarks>This is used for directional data on blocks like Furnaces</remarks>
        public static byte DirectionByRotationFlat(Entity p, bool invert = false)
        {
            byte direction = (byte)((int)Math.Floor((double)((p.Rotation.X * 4F) / 360F) + 0.5D) & 3);
            if (invert)
                switch (direction)
                {
                    case 0: return 2;
                    case 1: return 5;
                    case 2: return 3;
                    case 3: return 4;
                }
            else
                switch (direction)
                {
                    case 0: return 3;
                    case 1: return 4;
                    case 2: return 2;
                    case 3: return 5;
                }
            return 0;
        }

        /// <summary>
        /// Gets a byte representing block direction based on the rotation
        /// of the entity that placed it.
        /// </summary>
        /// <param name="p">The entity whose rotation should be used.</param>
        /// <param name="Position">The position of the block being placed.</param>
        /// <param name="invert">If set to <c>true</c>, the direction is inverted.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte DirectionByRotation(PlayerEntity p, Vector3 Position, bool invert = false)
        {
            double d = Math.Asin(((p.Location.Y + 0.5) - Position.Y) / Position.DistanceTo(p.Location + new Vector3(0, 0.5, 0)));
            if (d > (Math.PI / 4)) return invert ? (byte)1 : (byte)0;
            if (d < -(Math.PI / 4)) return invert ? (byte)0 : (byte)1;
            return DirectionByRotationFlat(p, invert);
        }

        /// <summary>
        /// Gets the distace between two points in 2D space.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double Distance2D(double A1, double A2, double B1, double B2)
        {
            return Math.Sqrt(Math.Pow(B1 - A1, 2) + Math.Pow(B2 - A2, 2));
        }
    }
}
