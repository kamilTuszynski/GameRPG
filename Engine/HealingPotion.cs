using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HealingPotion : Item 
    {        
        public int AmountToHeal { get; set; }

        public HealingPotion(int ID,string Name, int AmountToHeal, int Price):base(ID,Name,Price)
        {
            this.AmountToHeal = AmountToHeal;
        }
    }
}
