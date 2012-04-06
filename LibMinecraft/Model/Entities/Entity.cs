using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Server;

namespace LibMinecraft.Model.Entities
{
    /// <summary>
    /// A generalEntity
    /// </summary>
    /// <remarks></remarks>
    public abstract class Entity : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Entity()
        {
            _Location = Vector3.Zero;
            _Rotation = Vector3.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="Location">The location.</param>
        /// <remarks></remarks>
        public Entity(Vector3 Location)
        {
            this._Location = Location;
            this.Velocity = Vector3.Zero;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        /// <remarks></remarks>
        public int ID { get; set; }

        /// <summary>
        /// Gets the type ID.
        /// </summary>
        /// <remarks></remarks>
        public abstract byte TypeID { get; }

        /// <summary>
        /// Gets the collision box.
        /// </summary>
        /// <remarks></remarks>
        public abstract Cuboid CollisionBox { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Entity"/> is gravity.
        /// </summary>
        /// <remarks></remarks>
        public virtual bool Gravity
        {
            get
            {
                return true;
            }
        }

        internal Vector3 _Location;
        /// <summary>
        /// The location of the entity
        /// </summary>
        /// <value>The location.</value>
        /// <remarks></remarks>
        public Vector3 Location
        {
            get
            {
                return _Location;
            }
            set
            {
                OldLocation = _Location;
                if (!Equals(_Velocity, value) && PropertyChanged != null)
                {
                    _Location = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Location"));
                }
                else
                    _Location = value;
            }
        }

        /// <summary>
        /// Gets the block location.
        /// </summary>
        /// <remarks></remarks>
        public Vector3 BlockLocation
        {
            get
            {
                return (this.Location - this.CollisionBox.Size / 2).Floor();
            }
        }

        /// <summary>
        /// Gets or sets the old location.
        /// </summary>
        /// <value>The old location.</value>
        /// <remarks></remarks>
        public Vector3 OldLocation { get; set; }

        internal Vector3 _Rotation;
        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        /// <remarks></remarks>
        public Vector3 Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                if (!Equals(_Rotation, value))
                {
                    _Rotation = value;
                    FirePropertyChanged(this, new PropertyChangedEventArgs("Rotation"));
                }
                else
                    _Rotation = value;
            }
        }

        private Vector3 _HeadRotation;
        public Vector3 HeadRotation
        {
            get
            {
                return _HeadRotation;
            }
            set
            {
                if (!Equals(_HeadRotation, value))
                {
                    _HeadRotation = value;
                    FirePropertyChanged(this, new PropertyChangedEventArgs("HeadRotation"));
                }
                else
                    _HeadRotation = value;
            }
        }

        internal Vector3 _Velocity;
        /// <summary>
        /// Blocks per tick
        /// </summary>
        /// <value>The velocity.</value>
        /// <remarks></remarks>
        public Vector3 Velocity
        {
            get
            {
                return _Velocity;
            }
            set
            {
                if (!Equals(_Velocity, value) && PropertyChanged != null)
                {
                    _Velocity = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Velocity"));
                }
                else
                    _Velocity = value;
            }
        }

        internal Dimension _Dimension;
        /// <summary>
        /// Gets or sets the dimension.
        /// </summary>
        /// <value>The dimension.</value>
        /// <remarks></remarks>
        public Dimension Dimension
        {
            get
            {
                return _Dimension;
            }
            set
            {
                if (!Equals(_Dimension, value))
                {
                    _Dimension = value;
                    FirePropertyChanged(this, new PropertyChangedEventArgs("Dimension"));
                }
                else
                    _Dimension = value;
            }
        }

        internal int _WorldIndex;
        /// <summary>
        /// Gets or sets the index of the world.
        /// </summary>
        /// <value>The index of the world.</value>
        /// <remarks></remarks>
        public int LevelIndex
        {
            get
            {
                return _WorldIndex;
            }
            set
            {
                if (!Equals(_WorldIndex, value))
                {
                    _WorldIndex = value;
                    FirePropertyChanged(this, new PropertyChangedEventArgs("WorldIndex"));
                }
                else
                    _WorldIndex = value;
            }
        }

        /// <summary>
        /// Fires the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks></remarks>
        protected abstract void FirePropertyChanged(object sender, PropertyChangedEventArgs e);

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks></remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Ticks the specified world.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <remarks></remarks>
        public virtual void Tick(World world)
        {
            // TODO: Make this work better
            if (this is PlayerEntity)
                return; // Players handle their own physics
            if (this.Gravity)
                this.Velocity.Y -= 0.09; // Gravity is 0.49 meters/tick squared IRL, but faster in Minecraft
            if (this.Location.Y < 0)
            {
                world.RemoveEntity(this.ID);
                return;
            }
            Block b = world.GetBlock(this.Location + new Vector3(0, 
                this.CollisionBox.Height, 0));
            if (!(b is AirBlock) && !(b is WaterFlowingBlock) && !(b is LavaFlowingBlock) &&
                !(b is LavaStillBlock) && !(b is WaterStillBlock))
            {
                this.Velocity = Vector3.Zero;
                while (!(b is AirBlock) && !(b is WaterFlowingBlock) && !(b is LavaFlowingBlock) &&
                    !(b is LavaStillBlock) && !(b is WaterStillBlock))
                {
                    this.Location += Vector3.Up;
                    b = world.GetBlock(this.Location + new Vector3(0,
                        this.CollisionBox.Height, 0));
                }
                this.Location.Y = (int)this.Location.Y + 2;
            }
            else
                this.Location += this.Velocity;
        }
    }
}
