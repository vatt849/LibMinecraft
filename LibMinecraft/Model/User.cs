using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a Minecraft.net login
    /// </summary>
    public class User
    {
        /// <summary>
        /// The case-sensitive username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// This user's Minecraft.net password
        /// </summary>
        public string Password { get; set; }
    }
}
