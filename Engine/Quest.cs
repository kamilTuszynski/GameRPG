using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Quest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }
        public Item RewardItem { get; set; }
        public List<QuestFinishedItem> QuestFinishedItems { get; set; }

        public Quest(int ID,string Name,string Description,int RewardExperiencePoints,int RewardGold,Item RewardItem=null)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.RewardExperiencePoints = RewardExperiencePoints;
            this.RewardGold = RewardGold;
            this.RewardItem = RewardItem;
            QuestFinishedItems = new List<QuestFinishedItem>();
        }
    }
}
