using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public Item(int ID,string Name, int Price)
        {
            this.ID = ID;
            this.Name = Name;
            this.Price = Price;
        }
    }
}
