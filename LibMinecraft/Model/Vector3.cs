using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a location in the Minecraft world
    /// </summary>
    /// <remarks></remarks>
    public class Vector3 : IEquatable<Vector3>
    {
        /// <summary>
        /// The X coordinate
        /// </summary>
        public double X;
        /// <summary>
        /// The Y coordinate
        /// </summary>
        public double Y;
        /// <summary>
        /// The Z coordinate
        /// </summary>
        public double Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Vector3()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <remarks></remarks>
        public Vector3(double value)
        {
            X = Y = Z = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <remarks></remarks>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <remarks></remarks>
        public Vector3(Vector3 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public Vector3 Clone()
        {
            return (Vector3)this.MemberwiseClone();
        }

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtracts one vector from another.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Multiplies two vectors together.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        /// Divides one vector from another.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        /// <summary>
        /// Adds a double to all components of a vector.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator +(Vector3 a, double b)
        {
            return new Vector3(a.X + b, a.Y + b, a.Z + b);
        }

        /// <summary>
        /// Subtracts a double from all components of a vector.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator -(Vector3 a, double b)
        {
            return new Vector3(a.X - b, a.Y - b, a.Z - b);
        }

        /// <summary>
        /// Multiplies a double by all components of a vector.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator *(Vector3 a, double b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }

        /// <summary>
        /// Divides all components of a vector by a double.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Vector3 operator /(Vector3 a, double b)
        {
            return new Vector3(a.X / b, a.Y / b, a.Z / b);
        }

        /// <summary>
        /// Gets the integer modulus of two Vector3 values.
        /// Note: this is not a remainder, as is the usual use of %.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 operator %(Vector3 a, Vector3 b)
        {
            return new Vector3(((uint)a.X) % ((uint)b.X), ((uint)a.Y) % ((uint)b.Y), ((uint)a.Z) % ((uint)b.Z));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        /// <remarks></remarks>
        public bool Equals(Vector3 other)
        {
            if (other == null)
                return false;
            return other.X == this.X && other.Y == this.Y && other.Z == this.Z;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public override bool Equals(object obj)
        {
            return Equals((Vector3)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        /// <remarks></remarks>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        /// <summary>
        /// Gets the distance between this and another Vector3.
        /// </summary>
        /// <param name="other">The Vector3 to compare to.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public double DistanceTo(Vector3 other)
        {
            return Math.Sqrt(Math.Pow(other.Z - Z, 2) + Math.Sqrt(Math.Pow(other.X - X, 2)) + Math.Pow(other.Y - Y, 2));
        }

        /// <summary>
        /// Floors this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public Vector3 Floor()
        {
            return new Vector3((int)X, (int)Y, (int)Z);
        }

        /// <summary>
        /// Clamps to the specified minimum.
        /// </summary>
        /// <param name="Minimum">The minimum value.</param>
        /// <returns></returns>
        /// <remarks>Ensures that the absolute value of all components are less than or equal to the given minimum.</remarks>
        public Vector3 Clamp(double Minimum)
        {
            Minimum = Math.Abs(Minimum);
            Vector3 result = this.Clone();
            if (result.X < -Minimum)
                result.X = -Minimum;
            if (result.X > Minimum)
                result.X = Minimum;

            if (result.Y < -Minimum)
                result.Y = -Minimum;
            if (result.Y > Minimum)
                result.Y = Minimum;

            if (result.Z < -Minimum)
                result.Z = -Minimum;
            if (result.Z > Minimum)
                result.Z = Minimum;
            return result;
        }

        /// <summary>
        /// The toString function for Vector3.
        /// </summary>
        /// <returns>Returns a formatted vector string (X,Y,Z)</returns>
        public override string ToString()
        {
            return "<" + this.X + ", " + this.Y + ", " + this.Z + ">";
        }

        /// <summary>
        /// A Vector3 with all components set to 0.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 Zero
        {
            get
            {
                return new Vector3(0, 0, 0);
            }
        }
        /// <summary>
        /// A Vector3 with all components set to 1.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 One
        {
            get
            {
                return new Vector3(1, 1, 1);
            }
        }
        /// <summary>
        /// A Vector3 whose Z coordinate is set to 1.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 Forward
        {
            get
            {
                return new Vector3(0, 0, 1);
            }
        }
        /// <summary>
        /// A Vector3 whose Z coordinate is set to -1.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 Backward
        {
            get
            {
                return new Vector3(0, 0, -1);
            }
        }
        /// <summary>
        /// A Vector3 whose Y coordinate is set to 1.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 Up
        {
            get
            {
                return new Vector3(0, 1, 0);
            }
        }
        /// <summary>
        /// A Vector3 whose Y coordinate is set to -1.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 Down
        {
            get
            {
                return new Vector3(0, -1, 0);
            }
        }
        /// <summary>
        /// A Vector3 whose X coordinate is set to 1.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 Left
        {
            get
            {
                return new Vector3(-1, 0, 0);
            }
        }
        /// <summary>
        /// A Vector3 whose X coordinate is set to -1.
        /// </summary>
        /// <remarks></remarks>
        public static Vector3 Right
        {
            get
            {
                return new Vector3(1, 0, 0);
            }
        }

        /// <summary>
        /// A Vector3 whose Z coordinate is set to 1
        /// </summary>
        public static Vector3 South
        {
            get
            {
                return new Vector3(0, 0, 1);
            }
        }

        /// <summary>
        /// A Vector3 whose Z coordinate is set to -1
        /// </summary>
        public static Vector3 North
        {
            get
            {
                return new Vector3(0, 0, -1);
            }
        }

        /// <summary>
        /// A Vector3 whose X coordinate is set to -1
        /// </summary>
        public static Vector3 West
        {
            get
            {
                return new Vector3(-1, 0, 0);
            }
        }

        /// <summary>
        /// A Vector3 whose X coordinate is set to -1
        /// </summary>
        public static Vector3 East
        {
            get
            {
                return new Vector3(1, 0, 0);
            }
        }
    }
}
