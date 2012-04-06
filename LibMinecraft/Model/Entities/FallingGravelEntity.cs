using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Entities
{
    /// <summary>
    /// A Falling Gravel entity (TypeID = 71)
    /// </summary>
    /// <remarks></remarks>
    public class FallingGravelEntity : Entity, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the type ID (71)
        /// </summary>
        /// <remarks></remarks>
        public override byte TypeID
        {
            get { return 71; }
        }

        public FallingGravelEntity(Vector3 Location)
            : base(Location)
        {
            this.Location += this.CollisionBox.Size / 2;
            this.Velocity = Vector3.Zero;
        }

        public override Cuboid CollisionBox
        {
            get { return new Cuboid(this.Location, new Vector3(1)); }
        }

        /// <summary>
        /// The property changed event for FallingGravel Entity
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected override void FirePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }

        public override void Tick(World world)
        {
            base.Tick(world);
            if (this.Velocity.Y == 0)
            {
                world.RemoveEntity(this.ID);
                world.SetBlock(this.BlockLocation, new GravelBlock());
            }
        }
    }
}
