using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Monster> Monsters = new List<Monster>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();

        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURER_PASS = 10;

        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_GIANT_SPIDER = 3;

        public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;

        public const int UNSELLABLE_ITEM_PRICE = -1;

        static World()
        {
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "Zardzewiały miecz", 1, 10,5));
            Items.Add(new Item(ITEM_ID_RAT_TAIL, "Ogon szczura",5));
            Items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "Kawałek futra",10));
            Items.Add(new Item(ITEM_ID_SNAKE_FANG, "Kieł węża",15));
            Items.Add(new Item(ITEM_ID_SNAKESKIN, "Skóra węża",25));
            Items.Add(new Weapon(ITEM_ID_CLUB, "Pałka", 5, 10,30));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Mikstura uzdrawiająca", 20,15));
            Items.Add(new Item(ITEM_ID_SPIDER_FANG, "Kieł pająka",25));
            Items.Add(new Item(ITEM_ID_SPIDER_SILK, "Pajęczyna",20));
            Items.Add(new Item(ITEM_ID_ADVENTURER_PASS, "Przepustka podróżnika",UNSELLABLE_ITEM_PRICE));
        }

        private static void PopulateMonsters()
        {
            Monster rat = new Monster(MONSTER_ID_RAT, "Szczur", 5, 3, 10, 10, 10);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 75, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR), 75, true));

            Monster snake = new Monster(MONSTER_ID_SNAKE, "Wąż", 5, 3, 10, 3, 3);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

            Monster giantSpider = new Monster(MONSTER_ID_GIANT_SPIDER, "Olbrzymi pająk", 20, 5, 40, 10, 10);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));

            Monsters.Add(rat);
            Monsters.Add(snake);
            Monsters.Add(giantSpider);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden = new Quest(QUEST_ID_CLEAR_ALCHEMIST_GARDEN, "Zajmij się ogrodem alchemika",
                "Musisz zabić szczury w ogrodzie alchemika i przynieść 3 szczurze ogony. W nagrodę otrzymasz miksturę leczniczą i 20 sztuk złota",
                100, 20);

            clearAlchemistGarden.QuestFinishedItems.Add(new QuestFinishedItem(ItemByID(ITEM_ID_RAT_TAIL), 3));
            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

            Quest clearFarmersField = new Quest(QUEST_ID_CLEAR_FARMERS_FIELD, "Zajmij się polem farmera",
                "Musisz zabić węże na polu farmera i przynieść 3 kły węża. W nagrodę otrzymasz przepustkę podróżnika i 50 sztuk złota",
                250, 50);

            clearFarmersField.QuestFinishedItems.Add(new QuestFinishedItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));
            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);

            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmersField);
        }

        private static void PopulateLocations()
        {
            //Tworzenie lokacji
            Location home = new Location(LOCATION_ID_HOME, "Dom", "Twój dom.");

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "Rynek miasta", "Widzisz bazar i kupców zachwalających swoje towary.");

            Vendor VendorInTownSquare = new Vendor("Kenneth");
            VendorInTownSquare.AddItemToInventory(ItemByID(ITEM_ID_HEALING_POTION), 4);
            VendorInTownSquare.AddItemToInventory(ItemByID(ITEM_ID_SNAKESKIN), 2);

            townSquare.VendorWorkingHere = VendorInTownSquare;

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "Chata alchemika", "Na pólkach widzisz dużo dziwnych, nieznanych ci roślin.");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);

            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN, "Ogród alchemika", "Rośnie tu wiele ciekawych roślin.");
            alchemistsGarden.MonsterLivingHere = MonsterByID(MONSTER_ID_RAT);

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "Farma", "Widzisz niedużą farmę, przed która stoi jej właściciel, farmer.");
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD);

            Location farmersField = new Location(LOCATION_ID_FARM_FIELD, "Pole", "Widzisz rzędy rosnących warzyw.");
            farmersField.MonsterLivingHere = MonsterByID(MONSTER_ID_SNAKE);

            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Strażnica", "Widzisz groźnie wyglądającego strażnika",null,null,ItemByID(ITEM_ID_ADVENTURER_PASS));

            Location bridge = new Location(LOCATION_ID_BRIDGE, "Most", "Most nad rzeką.");

            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Las", "Drzewa oplatają pajęcze sieci.");
            spiderField.MonsterLivingHere = MonsterByID(MONSTER_ID_GIANT_SPIDER);

            // Link the locations together
            home.LocationToNorth = townSquare;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;

            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;

            farmersField.LocationToEast = farmhouse;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;

            alchemistsGarden.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;

            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;

            spiderField.LocationToWest = bridge;

            // Add the locations to the static list
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
        }

        //metody pozwalające na wyszukiwanie itemkow, potworow, questow i lokacji po ID i zwracają je
        public static Item ItemByID(int ID)
        {
            foreach(Item item in Items)
            {
                if(item.ID==ID)
                {
                    return item;
                }
            }
            return null;
        }

        public static Monster MonsterByID(int ID)
        {
            foreach (Monster monster in Monsters)
            {
                if (monster.ID == ID)
                {
                    return monster;
                }
            }
            return null;
        }

        public static Quest QuestByID(int ID)
        {
            foreach (Quest quest in Quests)
            {
                if (quest.ID == ID)
                {
                    return quest;
                }
            }
            return null;
        }
        
        public static Location LocationByID(int ID)
        {
            foreach (Location location in Locations)
            {
                if (location.ID == ID)
                {
                    return location;
                }
            }
            return null;
        }
        

    }
}
