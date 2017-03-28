using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class LootItem
    {
        public Item Details { get; set; }
        public int DropPercentage { get; set; }
        public bool IsDefaultItem { get; set; }

        public LootItem(Item Details,int DropPercentage,bool IsDefaultItem)
        {
            this.Details = Details;
            this.DropPercentage = DropPercentage;
            this.IsDefaultItem = IsDefaultItem;
        }

    }
}
