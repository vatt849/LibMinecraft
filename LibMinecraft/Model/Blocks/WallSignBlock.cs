using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model.Entities;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// A Sign block on a Wall (ID = 68)
    /// </summary>
    /// <remarks></remarks>
    public class WallSignBlock : Block
    {
        /// <summary>
        /// The Block ID for this block (68)
        /// </summary>
        /// <remarks></remarks>
        public override byte BlockID
        {
            get { return 68; }
        }

        public override BlockData AdditionalData 
        {
            get
            {
                return Data;
            }
        }

        /// <summary>
        /// Returns whether or not a block needs support from a block below it.
        /// Wall Signs need support.
        /// </summary>
        public override bool RequiresSupport
        {
            get { return true; }
        }

        public override Vector3 SupportingBlockDirection
        {
            get
            {
                switch (this.Metadata)
                {
                    case 2:
                        return Vector3.South;
                    case 3:
                        return Vector3.North;
                    case 4:
                        return Vector3.East;
                    case 5:
                        return Vector3.West;
                    default:
                        return Vector3.North;
                }
            }
        }

        public override BlockOpacity Transparent
        {
            get { return BlockOpacity.NonSolid; }
        }


        public LibMinecraft.Model.Blocks.SignPostBlock.SignData Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignPostBlock"/> class.
        /// </summary>
        public WallSignBlock()
        {
            Data = new LibMinecraft.Model.Blocks.SignPostBlock.SignData();
        }

        /// <summary>
        /// Called when this block is placed
        /// </summary>
        /// <param name="world">The world it was placed in</param>
        /// <param name="position">The position it was placed at</param>
        /// <param name="blockClicked">The location of the block left clicked upon placing</param>
        /// <param name="facing">The facing of the placement</param>
        /// <param name="placedBy">The entity who placed the block</param>
        /// <returns>
        /// False to override placement
        /// </returns>
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            this.Metadata = MathHelper.DirectionByRotationFlat(placedBy, true);
            return true;
        }
    }
}
