using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Model.Items
{
    public class GenericItem : Item
    {
        private short internalItemID;

        public override short ItemID
        {
            get { return internalItemID; }
        }

        public GenericItem(short ItemID)
        {
            this.internalItemID = ItemID;
        }
    }
}
