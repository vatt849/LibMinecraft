using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model.Entities
{
    /// <summary>
    /// A Falling Ender Dragon Egg entity (TypeID = 74)
    /// </summary>
    /// <remarks></remarks>
    public class FallingEnderDragonEggEntity : Entity, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the type ID (74)
        /// </summary>
        /// <remarks></remarks>
        public override byte TypeID
        {
            get { return 74; }
        }

        public FallingEnderDragonEggEntity(Vector3 Location)
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
        /// The event handler for changed properties in Falling Ender Dragon Egg
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
                world.SetBlock(this.BlockLocation, new EnderDragonEggBlock());
            }
        }
    }
}
