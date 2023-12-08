using MySqlConnector;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Database access class for the Player.
    /// </summary>
    public class PlayerDataAccess
    {
        /// <summary>
        /// Checks if a player with the same kit number already exists in the team within the database.
        /// </summary>
        /// <param name="kitNumber">The kit number to be checked.</param>
        /// <param name="teamID">The team ID to be checked.</param>
        /// <returns>True if a player already exists with that kit number in the team within the database or false if it doesn't.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to check if the kit number already exists in the team.</exception>
        public bool DoesKitNumberExistInTeam(int kitNumber, int teamID)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Players WHERE PlayerKitNumber = @KitNumber AND TeamID = @TeamID;";

                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@KitNumber", kitNumber);
                            command.Parameters.AddWithValue("@TeamID", teamID);

                            return Convert.ToInt32(command.ExecuteScalar()) > 0;
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to check if the kit number already exists in the team."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Adding a new player to the database.
        /// </summary>
        /// <param name="player">The player object to be added to the database.</param>
        /// <returns>True if the player has been added to the database or an exception if it couldn't be added.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to add the player to the database.</exception>"
        public bool AddToDatabase(Player player)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string insertQuery = "INSERT INTO Players (PlayerFirstName, PlayerLastName, PlayerAge, PlayerKitNumber, Position, TeamID, GoalsScored, Assists, CleanSheets, YellowCards, RedCards) " +
                                   "VALUES (@PlayerFirstName, @PlayerSurname, @PlayerAge, @PlayerKitNumber, @Position, @TeamID, @GoalsScored, @Assists, @CleanSheets, @YellowCards, @RedCards); SELECT LAST_INSERT_ID();";
                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                        {
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

                            player.PlayerID = Convert.ToInt32(command.ExecuteScalar());

                            // Return true if the player has been added to the database or throw an exception if it couldn't be added
                            return player.PlayerID > 0 ? true : throw new Exception("Failed to add the player to the database.");
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to add the league to the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Retrieves all player records for a team from the database.
        /// </summary>
        /// <param name="team">The team to retrieve the players for.</param>
        /// <returns>An ObservableCollection of players instances from a team populated with data from the database.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="Exception">A player from the database has an invalid ID.</exception>
        /// <exception cref="MySqlException">Failed to get all the players from the database.</exception>
        public ObservableCollection<Player> GetAllPlayersForTeamFromDatabase(Team team)
        {
            ObservableCollection<Player> players = new ObservableCollection<Player>();

            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string selectQuery = $"SELECT * FROM Players WHERE TeamID = @TeamID ORDER BY PlayerKitNumber ASC;";

                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            command.Parameters.AddWithValue("@TeamID", team.TeamID);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
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

                                    if (player.PlayerID > 0) { players.Add(player); }
                                    else { throw new Exception("A player retrieved from the database has an invalid ID."); }
                                }
                                reader.Close();
                                return players;
                            }
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to get all the players from the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Add a statistic to the database for a specific team.
        /// </summary>
        /// <param name="player">The player for the stats to be added to.</param>
        /// <param name="property">The property of the stat to be added.</param>
        /// <exception cref="ArgumentException">Invalid stat property name.</exception>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to add the stat to the database.</exception>
        public void AddStatisticToDatabase(Player player, PropertyInfo property)
        {
            // Validate that the property name matches a valid column name to prevent SQL injection.
            string[] validStats = new string[] { "GoalsScored", "Assists", "CleanSheets", "YellowCards", "RedCards" };
            string stat = property.Name.ToString();

            if (!validStats.Contains(stat))
            {
                throw new ArgumentException("Invalid stat property name.");
            }
            else
            {
                if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
                else
                {
                    try
                    {
                        using (MySqlConnection connection = DatabaseConnection.GetConnection())
                        {
                            string updateQuery = $"UPDATE Players SET {stat} = @StatValue WHERE PlayerID = @PlayerID;";

                            using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@StatValue", property.GetValue(player));
                                command.Parameters.AddWithValue("@PlayerID", player.PlayerID);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (MySqlException) { throw new Exception("Failed to add the stat to the database."); }
                    finally { DatabaseConnection.CloseConnection(); }
                }
            }
        }
    }
}
