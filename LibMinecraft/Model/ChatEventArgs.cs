using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;

namespace LibMinecraft.Model
{
    /// <summary>
    /// Represents a player chatting
    /// </summary>
    /// <remarks></remarks>
    public class ChatEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the raw text.
        /// </summary>
        /// <value>The raw text.</value>
        /// <remarks></remarks>
        public string RawText { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        /// <remarks></remarks>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        /// <remarks></remarks>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this message was sent privately.
        /// </summary>
        /// <value><c>true</c> if private; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Private { get; set; }
        /// <summary>
        /// When using LibMinecraft.Server, you
        /// can set this to true to stop the
        /// default action from occuring.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Handled { get; set; }
        /// <summary>
        /// Gets or sets the remote client.
        /// </summary>
        /// <value>The remote client.</value>
        /// <remarks></remarks>
        public RemoteClient RemoteClient { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatEventArgs"/> class.
        /// </summary>
        /// <param name="RawText">The raw text.</param>
        /// <remarks></remarks>
        public ChatEventArgs(string RawText)
        {
            this.RawText = RawText;
            this.Private = false;
            this.Handled = false;
            try
            {
                if (RawText.Contains(" whispers "))
                {
                    UserName = RawText.Remove(RawText.IndexOf(" whispers "));
                    Message = RawText.Substring(RawText.IndexOf(" whispers ") + " whispers ".Length);
                    Private = true;
                }
                else
                {
                    UserName = RawText.Remove(RawText.IndexOf(">"));
                    UserName = UserName.Substring(UserName.IndexOf("<") + 1);
                    Message = RawText.Substring(RawText.IndexOf(">") + 1);
                }

                for (int i = 0; i <= 0xF; i++)
                {
                    UserName = UserName.Replace("§" + i.ToString("x").ToLower(), "");
                    Message = Message.Replace("§" + i.ToString("x").ToLower(), "");
                }

                UserName = UserName.Trim();
                Message = Message.Trim();
            }
            catch { }
        }
    }
}
