using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel;

namespace Engine
{
    public class Player : LivingCreature
    {
        private int _gold;
        private int _experience;
        private Location _currentLocation;
        private Monster _monsterLivingHere;

                
        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPropertyChanged("Gold");
            }
        }
        public int Experience
        {
            get { return _experience; }
            private set
            {
                _experience = value;
                OnPropertyChanged("Experience");
                OnPropertyChanged("Level");
            }
        }        
        public int Level
        {
            get { return (Experience / 100) + 1; }
        }
        public Weapon CurrentWeapon { get; set; }
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }
        public BindingList<InventoryItem> Inventory { get; set; }
        public BindingList<PlayerQuest> Quests { get; set; }
        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }
        public List<HealingPotion> HealingPotions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList(); }
        }


        private Player(int Gold,int Experience, int CurrentHitPoints, int MaxHitPoints):base(CurrentHitPoints,MaxHitPoints)
        {
            this.Gold = Gold;
            this.Experience = Experience;
            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(0, 0, 40, 40);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_CLUB), 1));
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;
        }

        public void AddExperiencePoints(int experienceToAdd)
        {
            Experience += experienceToAdd;
            MaxHitPoints = 40 + 20 * (Level - 1);
        }

        public static Player CreatePlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerData = new XmlDocument();
                playerData.LoadXml(xmlPlayerData);

                int currentHP = Convert.ToInt32(playerData.SelectSingleNode("Player/Stats/CurrentHitPoints").InnerText);
                int maxHP = Convert.ToInt32(playerData.SelectSingleNode("Player/Stats/MaxHitPoints").InnerText);
                int experience = Convert.ToInt32(playerData.SelectSingleNode("Player/Stats/Experience").InnerText);
                int gold = Convert.ToInt32(playerData.SelectSingleNode("Player/Stats/Gold").InnerText);

                Player player = new Player(gold, experience, currentHP, maxHP);

                int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);

                if(playerData.SelectSingleNode("Player/Stats/CurrentWeapon")!=null)
                {
                    int currentWeaponID = Convert.ToInt32(playerData.SelectSingleNode("Player/Stats/CurrentWeapon").InnerText);
                    player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);
                }

                foreach(XmlNode node in playerData.SelectNodes("Player/InventoryItems/InventoryItem"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

                    for(int i=0;i<quantity;i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }

                foreach (XmlNode node in playerData.SelectNodes("Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);

                    PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(id));
                    playerQuest.IsCompleted = isCompleted;

                    player.Quests.Add(playerQuest);
                }

                return player;
            }

            catch
            {
                return Player.CreateDefaultPlayer();
            }
        }
        
        public bool HasRequiredItemToEnterLocation(Location location)
        {
            if(location.ItemRequiredToEnter==null)
            {
                //Nie ma itemka koniecznego, żeby wejść do lokacji, wiec zwracam true
                return true;
            }

            //Sprawdzamy czy grac ma ten itemek, zwraca true jak znajdzie item, false jak nie znajdzie            
            return Inventory.Any(item => item.Details.ID == location.ItemRequiredToEnter.ID);
        }

        public bool HasThisQuest(Quest quest)
        {            
            return Quests.Any(q => q.Details.ID == quest.ID);
        }

        public bool CompletedThisQuest(Quest quest)
        {
            foreach(PlayerQuest q in Quests)
            {
                if(q.Details.ID==quest.ID)
                {
                    return q.IsCompleted;
                }
            }
            return false;
        }

        public bool HasAllItemsToFinishQuest(Quest quest)
        {
            //Sprawdzamy czy gracz ma wszystkie potrzebne itemki
            foreach(QuestFinishedItem item in quest.QuestFinishedItems)
            {
                if(!Inventory.Any(i=>i.Details.ID==item.Details.ID && i.Quantity>=item.Quantity))
                {
                    return false;
                }
            }

            //jesli doszlismy tu to znaczy ze ma wszystkie itemki w odpowiedniej ilosci
            return true;
        }

        public void RemoveQuestFinishedItems(Quest quest)
        {
            foreach(QuestFinishedItem item in quest.QuestFinishedItems)
            {
                InventoryItem ii = Inventory.SingleOrDefault(i => i.Details.ID == item.Details.ID);

                if(ii!=null)
                {
                    RemoveItemFromInventory(ii.Details, item.Quantity);
                }
            }
        }

        public void AddItemToInventory(Item itemToAdd,int quantity=1)
        {
            InventoryItem ii = Inventory.SingleOrDefault(i => i.Details.ID == itemToAdd.ID);

            if(ii==null)
            {
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                ii.Quantity+=quantity;
            }
            RaiseInventoryChangedEvent(itemToAdd);
        }

        public void MarkQuestAsFinished(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(q => q.Details.ID == quest.ID);

            if(playerQuest!=null)
            {
                playerQuest.IsCompleted = true;
            }            
        }

        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();

            //glowny node
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);

            //child node do playera, ktora bedzie trzymala statystyki gracza
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);

            //tworzenie child node do stats
            XmlNode currentHitPoints = playerData.CreateElement("CurrentHitPoints");
            currentHitPoints.AppendChild(playerData.CreateTextNode(this.CurrentHitPoints.ToString()));
            stats.AppendChild(currentHitPoints);

            XmlNode maxHitPoints = playerData.CreateElement("MaxHitPoints");
            maxHitPoints.AppendChild(playerData.CreateTextNode(this.MaxHitPoints.ToString()));
            stats.AppendChild(maxHitPoints);

            XmlNode gold = playerData.CreateElement("Gold");
            gold.AppendChild(playerData.CreateTextNode(this.Gold.ToString()));
            stats.AppendChild(gold);

            XmlNode experience = playerData.CreateElement("Experience");
            experience.AppendChild(playerData.CreateTextNode(this.Experience.ToString()));
            stats.AppendChild(experience);

            XmlNode currentLocation = playerData.CreateElement("CurrentLocation");
            currentLocation.AppendChild(playerData.CreateTextNode(this.CurrentLocation.ID.ToString()));
            stats.AppendChild(currentLocation);

            //dostawalem czasami jakis wyjatek, chyba nullReferenceException
            if (CurrentWeapon!=null)
            {
                XmlNode currentWeapon = playerData.CreateElement("CurrentWeapon");
                currentWeapon.AppendChild(playerData.CreateTextNode(this.CurrentWeapon.ID.ToString()));
                stats.AppendChild(currentWeapon);
            }
            
            //koniec tworzenia child node do stats

            //tworzenie child node do playera do przechowywania ekwipunku
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            //dodanie do ekwpinku nod z itemkami
            foreach (InventoryItem item in this.Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = item.Details.ID.ToString();
                inventoryItem.Attributes.Append(idAttribute);

                XmlAttribute quantityAttribute = playerData.CreateAttribute("Quantity");
                quantityAttribute.Value = item.Quantity.ToString();
                inventoryItem.Attributes.Append(quantityAttribute);

                inventoryItems.AppendChild(inventoryItem);
            }

            //tworzenie child node do playera do przechowania questow
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            //dodanie do ekwpinku nod z itemkami
            foreach (PlayerQuest quest in this.Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = quest.Details.ID.ToString();
                playerQuest.Attributes.Append(idAttribute);

                XmlAttribute isCompletedAttribute = playerData.CreateAttribute("IsCompleted");
                isCompletedAttribute.Value = quest.IsCompleted.ToString();
                playerQuest.Attributes.Append(isCompletedAttribute);

                playerQuests.AppendChild(playerQuest);
            }

            return playerData.InnerXml;
        }

        private void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
                OnPropertyChanged("Weapons");
            if (item is HealingPotion)
                OnPropertyChanged("HealingPotions");
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(i => i.Details.ID == itemToRemove.ID);

            if(item==null)
            {
                //moze jakis blad, bo takiego itemka nie ma w ekwipunku
            }
            else
            {
                item.Quantity -= quantity;
                if (item.Quantity < 0)
                    item.Quantity = 0;
                if (item.Quantity == 0)
                    Inventory.Remove(item);
                RaiseInventoryChangedEvent(itemToRemove);
            }

        }

        public void MoveTo(Location location)
        {
            if (!HasRequiredItemToEnterLocation(location))
            {
                //wysiwetlamy wiadomosc i konczymy ruch, gracz nie moze tam wejsc
                string message = "Musisz mieć " + location.ItemRequiredToEnter.Name + ", aby móc wejść do tej lokacji.";
                RaiseMessage(message, true);
                return;
            }

            CurrentLocation = location;

            if (location.HasQuest)
            {
                bool playerAlreadyHasQuest = HasThisQuest(location.QuestAvailableHere);
                bool playerAlreadyFinishedQuest = CompletedThisQuest(location.QuestAvailableHere);

                if (playerAlreadyHasQuest)
                {
                    //jesli gracz ma ten quest i go nie ukonczyl
                    if (!playerAlreadyFinishedQuest)
                    {
                        bool playerAlreadyHasItemsToFinishQuest = HasAllItemsToFinishQuest(location.QuestAvailableHere);

                        if (playerAlreadyHasItemsToFinishQuest)
                        {
                            GivePlayerQuestRewards(location.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    GiveQuestToPlayer(location.QuestAvailableHere);
                }
            }

            SetTheCurrentMonsterForTheCurrentLocation(location);

        }

        private void SetTheCurrentMonsterForTheCurrentLocation(Location location)
        {
            if (location.MonsterLivingHere != null)
            {
                RaiseMessage("Widzisz " + location.MonsterLivingHere.Name + ". Musisz go pokonać.", true);

                //zespawnowanie potworka
                Monster standardMonster = World.MonsterByID(location.MonsterLivingHere.ID);

                _monsterLivingHere = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaxHitPoints);

                foreach (LootItem item in standardMonster.LootTable)
                {
                    _monsterLivingHere.LootTable.Add(item);
                }

            }
            else
            {
                _monsterLivingHere = null;
            }
        }

        private void GivePlayerQuestRewards(Quest questDone)
        {
            RaiseMessage("Ukończyłeś zadanie " + questDone.Name);
            RemoveQuestFinishedItems(questDone);

            //daj nagrody za questa
            RaiseMessage("Otrzymałeś " + questDone.RewardGold.ToString() +
                " sztuk złota nagrody za wykonanie zadania.");
            RaiseMessage("Otrzymałeś " + questDone.RewardExperiencePoints.ToString() +
                " punktów doświadczenia za wykonanie zadania.");
            RaiseMessage("Otrzymałeś " + questDone.RewardItem.Name.ToString() +
                " za wykonanie zadania.");

            Gold += questDone.RewardGold;
            AddExperiencePoints(questDone.RewardExperiencePoints);
            AddItemToInventory(questDone.RewardItem);
            MarkQuestAsFinished(questDone);
        }

        private void GiveQuestToPlayer(Quest questToGive)
        {
            RaiseMessage("Otrzymałeś zadanie" + questToGive.Name);
            RaiseMessage("Aby je wykonać musisz dostarczyć:");

            foreach (QuestFinishedItem item in questToGive.QuestFinishedItems)
            {
                RaiseMessage(item.Quantity.ToString() + " " + item.Details.Name);
            }
            RaiseMessage("", true);

            Quests.Add(new PlayerQuest(questToGive));
        }

        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
                MoveTo(CurrentLocation.LocationToNorth);
        }

        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
                MoveTo(CurrentLocation.LocationToSouth);
        }

        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
                MoveTo(CurrentLocation.LocationToEast);
        }

        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
                MoveTo(CurrentLocation.LocationToWest);
        }

        public void UseWeapon(Weapon weapon)
        {
            int damageToMonster = RandomNumberGenerator.NumberBetween(weapon.MinimumDamage, weapon.MaximumDamage);

            _monsterLivingHere.CurrentHitPoints -= damageToMonster;

            RaiseMessage("Zadałeś " + _monsterLivingHere.Name + " " + damageToMonster.ToString() + " punktów obrażeń");
            if(_monsterLivingHere.CurrentHitPoints>=0)
            {
                RaiseMessage(_monsterLivingHere.Name + " ma " + _monsterLivingHere.CurrentHitPoints.ToString() + "/" +
                _monsterLivingHere.MaxHitPoints.ToString() + " punktów życia", true);
            }
            else
            {
                RaiseMessage(_monsterLivingHere.Name + " ma 0/" + _monsterLivingHere.MaxHitPoints.ToString() + " punktów życia", true);
            }
            

            if (_monsterLivingHere.CurrentHitPoints <= 0)
            {
                RaiseMessage("Pokonałeś " + _monsterLivingHere.Name);
                RaiseMessage("Otrzymujesz " + _monsterLivingHere.RewardExperiencePoints.ToString() + " punktów doświadczenia oraz " +
                    _monsterLivingHere.RewardGold.ToString() + " sztuk złota.", true);
                
                Gold += _monsterLivingHere.RewardGold;
                AddExperiencePoints(_monsterLivingHere.RewardExperiencePoints);

                //itemki zlootowane z potworka
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                foreach (LootItem item in _monsterLivingHere.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= item.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(item.Details, 1));
                    }
                }

                //jesli nie dodalismy losowo zadnego itemka to dodaj domyslny
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem item in _monsterLivingHere.LootTable)
                    {
                        if (item.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(item.Details, 1));
                        }
                    }
                }

                //dodanie itemkow zlootowanych do ekwpiunku gracza
                foreach (InventoryItem item in lootedItems)
                {
                    AddItemToInventory(item.Details);
                    RaiseMessage("Otrzymałeś " + item.Quantity.ToString() + " " + item.Details.Name,true);
                }
                                
                MoveTo(CurrentLocation);
            }
            else
            {
                //potwor zyje
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _monsterLivingHere.MaximumDamage);
                RaiseMessage(_monsterLivingHere.Name + " zadał ci " + damageToPlayer.ToString() + " punktów obrażeń");

                CurrentHitPoints -= damageToPlayer;

                if (CurrentHitPoints <= 0)
                {
                    RaiseDeath(_monsterLivingHere.Name + " zabił cię. Przegrałeś!");
                }
            }



        }

        public void UsePotion(HealingPotion potion)
        {
            CurrentHitPoints += potion.AmountToHeal;

            if (CurrentHitPoints > MaxHitPoints)
            {
                CurrentHitPoints = MaxHitPoints;
            }

            RemoveItemFromInventory(potion);

            RaiseMessage("Wypiłeś " + potion.Name + ", która leczy" + potion.AmountToHeal.ToString() + " punktów życia.");

            //teraz jest tura potworka
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _monsterLivingHere.MaxHitPoints);
            RaiseMessage(_monsterLivingHere.Name + " zadał ci " + damageToPlayer.ToString() + " punktów obrażeń");

            CurrentHitPoints -= damageToPlayer;

            if (CurrentHitPoints <= 0)
            {
                RaiseDeath(_monsterLivingHere.Name + " zabił cię. Przegrałeś!");
            }

        }

        public event EventHandler<MessageEventArgs> OnMessage;

        private void RaiseMessage(string message,bool addNewLine=false)
        {
            if (OnMessage != null)
                OnMessage(this, new MessageEventArgs(message, addNewLine));
        }

        public event EventHandler<DeathMessageEventArgs> OnDeath;

        private void RaiseDeath(string message)
        {
            if (OnDeath != null)
                OnDeath(this, new DeathMessageEventArgs(message));
        }

        public static Player CreatePlayerFromDatabase(int currentHitPoints,int maxHitPoints,
            int gold, int experience)
        {
            Player player = new Player(gold, experience, currentHitPoints, maxHitPoints);

            return player;
        }


    }
}
