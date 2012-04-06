using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Used for selecting one or more blocks in the region
    /// </summary>
    /// <remarks></remarks>
    public class Cuboid : IEquatable<Cuboid>
    {
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        /// <remarks></remarks>
        public Vector3 Location { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        /// <remarks></remarks>
        public Vector3 Size { get; set; }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        /// <value>The X position.</value>
        /// <remarks></remarks>
        public double X
        {
            get { return Location.X; }
            set { Location.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        /// <value>The Y position.</value>
        /// <remarks></remarks>
        public double Y
        {
            get { return Location.Y; }
            set { Location.Y = value; }
        }
        /// <summary>
        /// Gets or sets the Z position.
        /// </summary>
        /// <value>The Z position.</value>
        /// <remarks></remarks>
        public double Z
        {
            get { return Location.Z; }
            set { Location.Z = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        /// <remarks></remarks>
        public double Height
        {
            get { return Size.X; }
            set { Size.X = value; }
        }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        /// <remarks></remarks>
        public double Width
        {
            get { return Size.Y; }
            set { Size.Y = value; }
        }
        /// <summary>
        /// Gets or sets the depth.
        /// </summary>
        /// <value>The depth.</value>
        /// <remarks></remarks>
        public double Depth
        {
            get { return Size.Z; }
            set { Size.Z = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cuboid"/> class. Default location 0, 0, 0. Default size 1, 1, 1.
        /// </summary>
        /// <remarks></remarks>
        public Cuboid() 
        {
            this.Size = Vector3.One;
            this.Location = Vector3.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cuboid"/> class.
        /// </summary>
        /// <param name="X">The X position.</param>
        /// <param name="Y">The Y position.</param>
        /// <param name="Z">The Z position.</param>
        /// <remarks></remarks>
        public Cuboid(double X, double Y, double Z)
        {
            this.Size = Vector3.One;
            this.Location = Vector3.Zero;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Cuboid"/> class.
        /// </summary>
        /// <param name="X">The X position.</param>
        /// <param name="Y">The Y position.</param>
        /// <param name="Z">The Z position.</param>
        /// <param name="Height">The height.</param>
        /// <param name="Width">The width.</param>
        /// <param name="Depth">The depth.</param>
        /// <remarks></remarks>
        public Cuboid(double X, double Y, double Z, double Height, double Width, double Depth)
        {
            this.Size = Vector3.One;
            this.Location = Vector3.Zero;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Height = Height;
            this.Width  = Width;
            this.Depth = Depth;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Cuboid"/> class.
        /// </summary>
        /// <param name="Location">The location.</param>
        /// <remarks></remarks>
        public Cuboid(Vector3 Location)
        {
            this.Location = Location;
            this.Size = Vector3.One;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Cuboid"/> class.
        /// </summary>
        /// <param name="Location">The location.</param>
        /// <param name="Size">The size.</param>
        /// <remarks></remarks>
        public Cuboid(Vector3 Location, Vector3 Size)
        {
            this.Location = Location;
            this.Size = Size;
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">The Cuboid to multiply.</param>
        /// <param name="b">The Cuboid to multiply by.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Cuboid operator *(Cuboid a, Cuboid b) { return new Cuboid(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.Height * b.Height, a.Width * b.Width, a.Depth * b.Depth); }
        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="a">The Cuboid to divide.</param>
        /// <param name="b">The Cuboid to divide by.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Cuboid operator /(Cuboid a, Cuboid b) { return new Cuboid(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.Height / b.Height, a.Width / b.Width, a.Depth / b.Depth); }
        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">The Cuboid to add to.</param>
        /// <param name="b">The Cuboid to add.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Cuboid operator +(Cuboid a, Cuboid b) { return new Cuboid(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.Height + b.Height, a.Width + b.Width, a.Depth + b.Depth); }
        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">The Cuboid to take from.</param>
        /// <param name="b">The Cuboid to take.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static Cuboid operator -(Cuboid a, Cuboid b) { return new Cuboid(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.Height - b.Height, a.Width - b.Width, a.Depth - b.Depth); }
        /// <summary>
        /// The default cuboid
        /// </summary>
        public static readonly Cuboid Default = new Cuboid(0, 0, 0, 1, 1, 1);

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="Other">The other.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool Equals(Cuboid Other)
        {
            return this.X == Other.X && Y == Other.Y && Z == Other.Z && Height == Other.Height && Width == Other.Width && Depth == Other.Depth;
        }

        /// <summary>
        /// Checks if Bubloid Intersects.
        /// </summary>
        /// <param name="b">The other cuboid to test.</param>
        /// <returns>True if they intersects, False Otherwise</returns>
        /// <remarks></remarks>
        public bool Intersects(Cuboid b)
        {
            Cuboid a = this;
            
            return
                (this.X > b.X && this.X < b.X + b.Width) || (b.X > this.X && b.X < this.X + this.Width) &&
                (this.Y > b.Y && this.Y < b.Y + b.Width) || (b.Y > this.Y && b.Y < this.Y + this.Height) &&
                (this.Z > b.Z && this.Z < b.Z + b.Width) || (b.X > this.Z && b.Z < this.Z + this.Depth);

        }
        /// <summary>
        /// Checks if Location Intersects.
        /// </summary>
        /// <param name="Location">The location.</param>
        /// <returns>True if Location intersects, false otherwise</returns>
        /// <remarks></remarks>
        public bool Intersects(Vector3 Location)
        {
            return Intersects(new Cuboid(Location));
        }

        #region extra
        /// <summary>
        /// Gets the high point.
        /// </summary>
        /// <returns>The Vector3 of the highest Point of the Cuboid</returns>
        /// <remarks></remarks>
        public Vector3 GetHighPoint()
        {
            Vector3 r = new Vector3();
            //how is it measured, is the size off the corner or is the size of the area offset from all sides
            //for now it is set as the size off the corner

            r.X = X + Height - 1;
            r.Y = Y + Width - 1;
            r.Z = Z + Depth - 1;
            return r;
        }

        /// <summary>
        /// Gets the low point.
        /// </summary>
        /// <returns>The Vector3 of the lowest Point of the cuboid</returns>
        /// <remarks></remarks>
        public Vector3 GetLowPoint()
        {
            Vector3 R = new Vector3();
            //how is it measured, is the size off the corner or is the size of the area offset from all sides
            //for now it is set as the size off the corner

            R.X = X;
            R.Y = Y + this.Height;
            R.Z = Z;
            return R;
        }
        #endregion
    }
}
