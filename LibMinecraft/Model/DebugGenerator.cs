using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Model
{
    public class DebugGenerator : IWorldGenerator
    {
        public string Name
        {
            get { return "FLAT"; }
        }

        public int Version
        {
            get { return 1; }
        }

        public bool GenerateStructures { get { return false; } set { } }

        public MapColumn GenerateColumn(Vector3 Coordinates, Region Region, Dimension Dimension, long Seed)
        {
            MapColumn mc = new MapColumn(Region, Coordinates);
            // Floor of glass
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    mc.SetBlock(new Vector3(x, 0, z), new GlassBlock());
                    mc.SetBlock(new Vector3(x, 15, z), new GlassBlock());
                }
                //mc.SetBlock(new Vector3(x, 0, 0), new WoolBlock(Wool.Red));
                //mc.SetBlock(new Vector3(x, 0, 15), new WoolBlock(Wool.Red));
                //mc.SetBlock(new Vector3(0, 0, x), new WoolBlock(Wool.Red));
                //mc.SetBlock(new Vector3(15, 0, x), new WoolBlock(Wool.Red));
                mc.SetBlock(new Vector3(15, 0, x), new WoolBlock(Wool.Pink));
            }
            return mc;
        }
    }
}
