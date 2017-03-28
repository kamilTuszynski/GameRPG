using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Weapon : Item
    {        
        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        public Weapon(int ID,string Name, int MinimumDamage, int MaximumDamage):base(ID,Name)
        {
            this.MinimumDamage = MinimumDamage;
            this.MaximumDamage = MaximumDamage;
        }
    }
}
