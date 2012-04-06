using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// An abstract model for strongly-typed
    /// representation of blocks
    /// </summary>
    public abstract class Block
    {
        #region Block list

        private static Block[] Blocks = new Block[]
        {
            new AirBlock(), //0x00
            new StoneBlock(), //0x01
            new GrassBlock(), //0x02
            new DirtBlock(), //0x03
            new CobblestoneBlock(), //0x04
            new WoodenPlankBlock(), //0x05
            new SaplingBlock(), //0x06
            new BedrockBlock(), //0x07
            new WaterBlock(), //0x08
            new StationaryWaterBlock(), //0x09
            new LavaBlock(), //0x0a
            new StationaryLavaBlock(), //0x0b
            new SandBlock(), //0x0c
            new GravelBlock(), //0x0d
            new GoldOreBlock(), //0x0e
            new IronOreBlock(), //0x0f
            new CoalOreBlock(), //0x10
            new WoodBlock(), //0x11
            new LeafBlock(), //0x12
            new SpongeBlock(), //0x13
            new GlassBlock(), //0x14
            new RedClothBlock(), //0x15
            new OrangeClothBlock(), //0x16
            new YellowClothBlock(), //0x17
            new LimeClothBlock(), //0x18
            new GreenClothBlock(), //0x19
            new AquaGreenClothBlock(), //0x1a
            new CyanClothBlock(), //0x1b
            new BlueClothBlock(), //0x1c
            new PurpleClothBlock(), //0x1d
            new IndigoClothBlock(), //0x1e
            new VioletClothBlock(), //0x1f
            new MagentaClothBlock(), //0x20
            new PinkClothBlock(), //0x21
            new BlackClothBlock(), //0x22
            new GrayClothBlock(), //0x23
            new WhiteClothBlock(), //0x24
            new DandelionBlock(), //0x25
            new RoseBlock(), //0x26
            new BrownMushroomBlock(), //0x27
            new RedMushroomBlock(), //0x28
            new GoldBlock(), //0x29
            new IronBlock(), //0x2a
            new DoubleSlabBlock(), //0x2b
            new SlabBlock(), //0x2c
            new BrickBlock(), //0x2d
            new TNTBlock(), //0x2e
            new BookshelfBlock(), //0x2f
            new MossyCobblestoneBlock(), //0x30
            new ObsidianBlock(), //0x31
        };

        #endregion

        /// <summary>
        /// The Block ID for this block
        /// </summary>
        public abstract byte BlockID { get; }

        /// <summary>
        /// This block's friendly name for client inventories.
        /// By default, returns the name of the Type.
        /// </summary>
        public virtual string BlockName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        /// <summary>
        /// Converts a Block to a byte
        /// </summary>
        public static implicit operator byte(Block b)
        {
            return b.BlockID;
        }

        /// <summary>
        /// Converts a byte to a Block
        /// </summary>
        public static implicit operator Block(byte b)
        {
            Block bl = Blocks[b];
            return (Block)Activator.CreateInstance(bl.GetType());
        }

        #region Overrides

        /// <summary>
        /// Called when this block is placed
        /// </summary>
        /// <param name="world">The world it was placed in</param>
        /// <param name="position">The position it was placed at</param>
        /// <param name="blockClicked">The location of the block left clicked upon placing</param>
        /// <param name="facing">The facing of the placement</param>
        /// <returns>False to override placement</returns>
        /// <remarks></remarks>
        public virtual bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing)
        {
            return true;
        }

        /// <summary>
        /// Called when this block is destroyed
        /// </summary>
        /// <param name="world">The world the block used to exist in</param>
        /// <param name="position">The position the block used to occupy</param>
        /// <remarks></remarks>
        public virtual void BlockDestroyed(World world, Vector3 position)
        {
        }

        #endregion
    }
}
