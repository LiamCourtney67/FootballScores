using MySqlConnector;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace FootballScoresUI.models
{
    public class PlayerDataAccess
    {
        /// <summary>
        /// Checks if a team with the same name already exists in the league within the database
        /// </summary>
        /// <param name="teamName">The team name to be checked</param>
        /// <param name="leagueID">The ID of the league to be checked against</param>
        /// <returns>True if a team already exists with that name in the league within the database or false if it doesn't</returns>
        /// <exception cref="Exception"></exception>
        private bool DoesKitNumberExistInTeam(int kitNumber, int teamID)
        {
            try
            {
                // Attempt to open the database connection
                DatabaseConnection.OpenConnection();

                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    // SQL query to check if a player with the same kit number exists in the team
                    string checkQuery = "SELECT COUNT(*) FROM Players WHERE PlayerKitNumber = @KitNumber AND TeamID = @TeamID;";

                    using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                    {
                        // Add the kit number and teamID as a parameter to the SQL query
                        command.Parameters.AddWithValue("@KitNumber", kitNumber);
                        command.Parameters.AddWithValue("@TeamID", teamID);

                        try
                        {
                            // Execute the SQL query and get the count
                            int count = Convert.ToInt32(command.ExecuteScalar());

                            return count > 0;
                        }
                        catch (MySqlException e)
                        {
                            throw new Exception("Failed to check if the kit number already exists in the team within the database: " + e.Message + " " + e.InnerException);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to open the database connection: " + e.Message + " " + e.InnerException);
            }
        }

        /// <summary>
        /// Adding a new player to the database
        /// </summary>
        /// <param name="player">The player object to be added to the database</param>
        /// <returns>True if the player has been added to the database or an exception if it couldn't be added</returns>
        /// <exception cref="Exception"></exception>
        public bool AddToDatabase(Player player)
        {
            // Check if a team with the same name already exists in the league
            if (DoesKitNumberExistInTeam(player.KitNumber, player.Team.TeamID))
            {
                throw new Exception("A player with the same kit number already exists in the team.");
            }

            try
            {
                // Attempt to open the database connection
                DatabaseConnection.OpenConnection();

                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    // SQL query to insert a new player and get its ID
                    string insertQuery = "INSERT INTO Players (PlayerFirstName, PlayerLastName, PlayerAge, PlayerKitNumber, Position, TeamID, GoalsScored, Assists, CleanSheets, YellowCards, RedCards) " +
                               "VALUES (@PlayerFirstName, @PlayerSurname, @PlayerAge, @PlayerKitNumber, @Position, @TeamID, @GoalsScored, @Assists, @CleanSheets, @YellowCards, @RedCards); SELECT LAST_INSERT_ID();";
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        // Add the player parameters to the SQL query
                        command.Parameters.AddWithValue("@PlayerFirstName", player.FirstName);
                        command.Parameters.AddWithValue("@PlayerSurname", player.LastName);
                        command.Parameters.AddWithValue("@PlayerAge", player.Age);
                        command.Parameters.AddWithValue("@PlayerKitNumber", player.KitNumber);
                        command.Parameters.AddWithValue("@Position", player.Position);
                        command.Parameters.AddWithValue("@TeamID", player.Team.TeamID);
                        command.Parameters.AddWithValue("@GoalsScored", player.GoalsScored);
                        command.Parameters.AddWithValue("@Assists", player.Assists);
                        command.Parameters.AddWithValue("@CleanSheets", player.CleanSheets);
                        command.Parameters.AddWithValue("@YellowCards", player.YellowCards);
                        command.Parameters.AddWithValue("@RedCards", player.RedCards);

                        // Execute the SQL query and get the new player's ID
                        player.PlayerID = Convert.ToInt32(command.ExecuteScalar());

                        // Return true if the player has been added to the database or throw an exception if it couldn't be added
                        return player.PlayerID > 0 ? true : throw new Exception("Failed to add the player to the database.");
                    }
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("Failed to execute the insert query or retrieve the PlayerID: " + e.Message + " " + e.InnerException);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to interact with the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Close the database connection when done
                DatabaseConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves all player records for a team from the database.
        /// </summary>
        /// <param name="team">The team to retrieve players for.</param>
        /// <returns>A ObservableCollection of player instances from a team populated with data from the database.</returns>
        public ObservableCollection<Player> GetAllPlayersForTeamFromDatabase(Team team)
        {
            // Initialize a ObservableCollection to store and return the Player objects.
            ObservableCollection<Player> players = new ObservableCollection<Player>();

            // Attempt to open a connection to the database.
            if (!DatabaseConnection.OpenConnection())
            {
                // Throw an exception if the connection attempt fails.
                throw new Exception("Failed to open the database connection.");
            }

            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    // Define a SQL query to retrieve all player records for a team.
                    string selectQuery = $"SELECT * FROM Players WHERE TeamID = @TeamID ORDER BY PlayerKitNumber ASC;";

                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        // Add the team id as a parameter to the SQL query.
                        command.Parameters.AddWithValue("@TeamID", team.TeamID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {

                            // Iterate over the result set and populate the Player objects.
                            while (reader.Read())
                            {
                                Player player = new Player(
                                        Convert.ToInt32(reader["PlayerID"]),
                                        reader["PlayerFirstName"].ToString(),
                                        reader["PlayerLastName"].ToString(),
                                        Convert.ToInt32(reader["PlayerAge"]),
                                        Convert.ToInt32(reader["PlayerKitNumber"]),
                                        reader["Position"].ToString(),
                                        team,
                                        Convert.ToInt32(reader["GoalsScored"]),
                                        Convert.ToInt32(reader["Assists"]),
                                        Convert.ToInt32(reader["CleanSheets"]),
                                        Convert.ToInt32(reader["YellowCards"]),
                                        Convert.ToInt32(reader["RedCards"])
                                    );

                                // Validate the player object (i.e., ensure it has a valid ID) and add it to the ObservableCollection.
                                if (player.PlayerID > 0)
                                {
                                    players.Add(player);
                                }
                                else
                                {
                                    throw new Exception("A player retrieved from the database has an invalid ID.");
                                }
                            }
                            reader.Close();
                            return players;
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle any MySql-specific exceptions that might arise during database operations.
                throw new Exception("An error occurred while retrieving players from the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Ensure the database connection is closed after the operation.
                DatabaseConnection.CloseConnection();
            }
        }


        public void AddStatisticToDatabase(Player player, PropertyInfo property)
        {
            // Validate that the property name matches a valid column name to prevent SQL injection.
            string[] validStats = new string[] { "GoalsScored", "Assists", "CleanSheets", "YellowCards", "RedCards" };
            string stat = property.Name.ToString();

            if (!validStats.Contains(stat))
            {
                throw new ArgumentException("Invalid stat property name.");
            }

            // Attempt to open a connection to the database.
            if (!DatabaseConnection.OpenConnection())
            {
                // Throw an exception if the connection attempt fails.
                throw new Exception("Failed to open the database connection.");
            }

            try
            {
                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    // Define a SQL query to update a specific player record.
                    string updateQuery = $"UPDATE Players SET {stat} = @StatValue WHERE PlayerID = @PlayerID;";

                    using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                    {
                        // Add the stat value and playerID as parameters to the SQL query.
                        command.Parameters.AddWithValue("@StatValue", property.GetValue(player));
                        command.Parameters.AddWithValue("@PlayerID", player.PlayerID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle any MySql-specific exceptions that might arise during database operations.
                throw new Exception("An error occurred while updating player stats to the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Ensure the database connection is closed after the operation.
                DatabaseConnection.CloseConnection();
            }
        }
    }
}
