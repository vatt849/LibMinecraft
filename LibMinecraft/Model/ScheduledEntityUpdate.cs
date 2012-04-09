using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model
{
    public class ScheduledEntityUpdate : IScheduledUpdate
    {
        public Entity Entity;
        public World World;

        public ScheduledEntityUpdate(Entity Entity, World World)
        {
            this.Entity = Entity;
            this.World = World;
        }

        public void Update()
        {
            Entity.ScheduledUpdate(World);
        }

        public long TicksRemaining { get; set; }
    }
}
