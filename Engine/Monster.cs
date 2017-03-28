using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Monster : LivingCreature
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaximumDamage { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }
        public List<LootItem> LootTable { get; set; }

        public Monster(int ID,string Name,int MaximumDamage,int RewardExperiencePoints,int RewardGold,int CurrentHitPoints,int MaxHitPoints):
            base(CurrentHitPoints,MaxHitPoints)
        {
            this.ID = ID;
            this.Name = Name;
            this.MaximumDamage = MaximumDamage;
            this.RewardExperiencePoints = RewardExperiencePoints;
            this.RewardGold = RewardGold;
            LootTable = new List<LootItem>();
        }
    }
}
