using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNbt;

namespace LibMinecraft.Model.Blocks
{
    /// <summary>
    /// The data relating to the blocks
    /// </summary>
    /// <remarks></remarks>
    public abstract class BlockData
    {
        /// <summary>
        /// Gets the block ID.
        /// </summary>
        /// <remarks></remarks>
        public abstract byte BlockID { get; }

        /// <summary>
        /// Serializes this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract byte[] Serialize();
        /// <summary>
        /// Deseralizes the specified data.
        /// </summary>
        /// <param name="Data">The data.</param>
        /// <remarks></remarks>
        public abstract void Deseralize(byte[] Data);

        /// <summary>
        /// Writes the NBT.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract NbtFile WriteNBT();
        /// <summary>
        /// Reads the NBT.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public abstract NbtFile ReadNBT();

        /// <summary>
        /// Gets weather or not to send with chunks.
        /// </summary>
        /// <remarks></remarks>
        public virtual bool SendWithChunks
        {
            get
            {
                return true;
            }
        }
    }
}
