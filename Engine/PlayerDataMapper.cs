using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Engine
{
    public static class PlayerDataMapper
    {
        /*private static readonly string _conncetionString =
            "Data Source=(local);Initial Catalog=GameRPG;Integrated Security=True";*/
        private static readonly string _conncetionString =
            @"Server=.\SQLEXPRESS;Database=GameRPG;Trusted_Connection=True;";

        public static Player CreateFromDatabase()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_conncetionString))
                {
                    connection.Open();

                    Player player;

                    using (SqlCommand savedGameCommand = connection.CreateCommand())
                    {
                        savedGameCommand.CommandType = CommandType.Text;
                        savedGameCommand.CommandText = "SELECT TOP 1 * FROM SavedGame";

                        SqlDataReader reader = savedGameCommand.ExecuteReader();

                        //nie ma zadnych danych w tabeli dlatego null
                        if (!reader.HasRows)
                            return null;

                        reader.Read();

                        int currentHitPoints = (int)reader["CurrentHitPoints"];
                        int maxHitPoints = (int)reader["MaxHitPoints"];
                        int gold = (int)reader["Gold"];
                        int experience = (int)reader["Experience"];
                        int currentLocationID = (int)reader["CurrentLocationID"];

                        player = Player.CreatePlayerFromDatabase(currentHitPoints, maxHitPoints, gold, experience);

                        reader.Close();
                    }

                    using (SqlCommand questCommand = connection.CreateCommand())
                    {
                        questCommand.CommandType = CommandType.Text;
                        questCommand.CommandText = "SELECT * FROM Quest";

                        SqlDataReader reader = questCommand.ExecuteReader();

                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                int questID = (int)reader["QuestID"];
                                bool isCompleted = (bool)reader["IsCompleted"];

                                PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(questID));
                                playerQuest.IsCompleted = isCompleted;

                                player.Quests.Add(playerQuest);
                            }
                        }
                        reader.Close();
                    }

                    using (SqlCommand inventoryCommand = connection.CreateCommand())
                    {
                        inventoryCommand.CommandType = CommandType.Text;
                        inventoryCommand.CommandText = "SELECT * FROM Inventory";

                        SqlDataReader reader = inventoryCommand.ExecuteReader();

                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                int inventoryItemID = (int)reader["InventoryItemID"];
                                int quantity = (int)reader["Quantity"];

                                player.AddItemToInventory(World.ItemByID(inventoryItemID), quantity);
                            }
                        }
                        reader.Close();
                    }

                    using (SqlCommand savedGameCommand = connection.CreateCommand())
                    {
                        savedGameCommand.CommandType = CommandType.Text;
                        savedGameCommand.CommandText = "SELECT TOP 1 * FROM SavedGame";

                        SqlDataReader reader = savedGameCommand.ExecuteReader();
                                                
                        reader.Read();
                                                
                        int currentLocationID = (int)reader["CurrentLocationID"];

                        player.MoveTo(World.LocationByID(currentLocationID));

                        reader.Close();
                    }

                    //stworzylismy gracza
                    return player;
                }
            }
            catch(Exception ex)
            {
                //bledy
            }
            return null;
        }

        public static void SaveToDatabase(Player player)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_conncetionString))
                {
                    connection.Open();

                    using (SqlCommand existingRowCountCommand = connection.CreateCommand())
                    {
                        existingRowCountCommand.CommandType = CommandType.Text;
                        existingRowCountCommand.CommandText = "SELECT COUNT(*) FROM SavedGame";

                        int existingRowCount = (int)existingRowCountCommand.ExecuteScalar();

                        if(existingRowCount==0)
                        {
                            //tabela nie ma wierszy wiec INSERT
                            using (SqlCommand insertSavedGame = connection.CreateCommand())
                            {
                                insertSavedGame.CommandType = CommandType.Text;
                                insertSavedGame.CommandText =
                                    "INSERT INTO SavedGame " +
                                    "(CurrentHitPoints, MaxHitPoints, Gold, Experience, CurrentLocationID) " +
                                    "VALUES " +
                                    "(@CurrentHitPoints, @MaxHitPoints, @Gold, @Experience, @CurrentLocationID)";

                                insertSavedGame.Parameters.Add("@CurrentHitPoints", SqlDbType.Int);
                                insertSavedGame.Parameters["@CurrentHitPoints"].Value = player.CurrentHitPoints;
                                insertSavedGame.Parameters.Add("@MaxHitPoints", SqlDbType.Int);
                                insertSavedGame.Parameters["@MaxHitPoints"].Value = player.MaxHitPoints;
                                insertSavedGame.Parameters.Add("@Gold", SqlDbType.Int);
                                insertSavedGame.Parameters["@Gold"].Value = player.Gold;
                                insertSavedGame.Parameters.Add("@Experience", SqlDbType.Int);
                                insertSavedGame.Parameters["@Experience"].Value = player.Experience;
                                insertSavedGame.Parameters.Add("@CurrentLocationID", SqlDbType.Int);
                                insertSavedGame.Parameters["@CurrentLocationID"].Value = player.CurrentLocation.ID;

                                insertSavedGame.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            //tabela ma wiersz wiec UPDATE
                            using (SqlCommand updateSavedGame = connection.CreateCommand())
                            {
                                updateSavedGame.CommandType = CommandType.Text;
                                updateSavedGame.CommandText =
                                    "UPDATE SavedGame " +
                                    "SET CurrentHitPoints = @CurrentHitPoints, " +
                                    "MaxHitPoints = @MaxHitPoints, " +
                                    "Gold = @Gold, " +
                                    "Experience = @Experience, " +
                                    "CurrentLocationID = @CurrentLocationID";

                                updateSavedGame.Parameters.Add("@CurrentHitPoints", SqlDbType.Int);
                                updateSavedGame.Parameters["@CurrentHitPoints"].Value = player.CurrentHitPoints;
                                updateSavedGame.Parameters.Add("@MaxHitPoints", SqlDbType.Int);
                                updateSavedGame.Parameters["@MaxHitPoints"].Value = player.MaxHitPoints;
                                updateSavedGame.Parameters.Add("@Gold", SqlDbType.Int);
                                updateSavedGame.Parameters["@Gold"].Value = player.Gold;
                                updateSavedGame.Parameters.Add("@Experience", SqlDbType.Int);
                                updateSavedGame.Parameters["@Experience"].Value = player.Experience;
                                updateSavedGame.Parameters.Add("@CurrentLocationID", SqlDbType.Int);
                                updateSavedGame.Parameters["@CurrentLocationID"].Value = player.CurrentLocation.ID;

                                updateSavedGame.ExecuteNonQuery();
                            }
                        }
                    }
                    ////////////////////usuwamy quest
                    using (SqlCommand deleteQuestCommand = connection.CreateCommand())
                    {
                        deleteQuestCommand.CommandType = CommandType.Text;
                        deleteQuestCommand.CommandText = "DELETE FROM Quest";
                        deleteQuestCommand.ExecuteNonQuery();
                    }

                    foreach(PlayerQuest playerQuest in player.Quests)
                    {
                        using (SqlCommand insertQuestCommand = connection.CreateCommand())
                        {
                            insertQuestCommand.CommandType = CommandType.Text;
                            insertQuestCommand.CommandText="INSERT INTO Quest (QuestID, IsCompleted) VALUES (@QuestID, @IsCompleted)";

                            insertQuestCommand.Parameters.Add("@QuestID", SqlDbType.Int);
                            insertQuestCommand.Parameters["@QuestID"].Value = playerQuest.Details.ID;
                            insertQuestCommand.Parameters.Add("@IsCompleted", SqlDbType.Bit);
                            insertQuestCommand.Parameters["@IsCompleted"].Value = playerQuest.IsCompleted;

                            insertQuestCommand.ExecuteNonQuery();
                        }
                    }
                    /////////////// usuwamy ekwpiunek
                    using (SqlCommand deleteInventoryCommand = connection.CreateCommand())
                    {
                        deleteInventoryCommand.CommandType = CommandType.Text;
                        deleteInventoryCommand.CommandText = "DELETE FROM Inventory";
                        deleteInventoryCommand.ExecuteNonQuery();
                    }

                    foreach (InventoryItem inventoryItem in player.Inventory)
                    {
                        using (SqlCommand insertInventoryCommand = connection.CreateCommand())
                        {
                            insertInventoryCommand.CommandType = CommandType.Text;
                            insertInventoryCommand.CommandText =
                                "INSERT INTO Inventory (InventoryItemID, Quantity) VALUES (@InventoryItemID, @Quantity)";

                            insertInventoryCommand.Parameters.Add("@InventoryItemID", SqlDbType.Int);
                            insertInventoryCommand.Parameters["@InventoryItemID"].Value = inventoryItem.Details.ID;
                            insertInventoryCommand.Parameters.Add("@Quantity", SqlDbType.Int);
                            insertInventoryCommand.Parameters["@Quantity"].Value = inventoryItem.Quantity;

                            insertInventoryCommand.ExecuteNonQuery();
                        }
                    }


                }
            }
            catch(Exception ex)
            {
                //ignoruje bledy narazie
            }
        }


    }
}
