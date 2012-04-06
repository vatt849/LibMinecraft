using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{

    /// <summary>
    /// A Single Slab (Half - 44)
    /// </summary>
    public class HalfSlabBlock : Block
    {
        /// <summary>
        /// The Block ID for this block
        /// </summary>
        public override byte BlockID
        {
            get { return 44; }
        }

        
        public override byte Metadata
        {
            get
            {
                return base.Metadata;
            }
            set
            {
                base.Metadata = value;
            }
        }
        // TODO: Single Slab
        public HalfSlabBlock()
        {

        }

        /// <summary>
        /// Called when this block is placed - turns to full slab if placed on half slab
        /// </summary>
        /// <param name="world">The world it was placed in</param>
        /// <param name="position">The position it was placed at</param>
        /// <param name="clickedBy">The entity who clicked the block</param>
        /// <returns>
        /// False to override placement
        /// </returns>
        public override bool BlockRightClicked(World world, Vector3 position, PlayerEntity clickedBy)
        {
            //TODO: Place in if statement to see if slab is being held
            if (world.GetBlock(new Vector3(position.X, position.Y, position.Z)) is HalfSlabBlock)
            {
                world.SetBlock(new Vector3(position.X, position.Y, position.Z), new SlabBlock());
            }
            return false;
        }



        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonCubeSolid; }
        }
    }
}