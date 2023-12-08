using MySqlConnector;
using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Database access class for the League.
    /// </summary>
    public class LeagueDataAccess
    {
        /// <summary>
        /// Checks if a league with the same name already exists in the database.
        /// </summary>
        /// <param name="leagueName">The league name to be checked.</param>
        /// <returns>True if a team already exists with that name in the database or false if it doesn't.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Falied to check if the league name already exists</exception>
        public bool DoesLeagueNameExist(string leagueName)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Leagues WHERE LeagueName = @LeagueName;";

                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@LeagueName", leagueName);

                            // Return true if the league name already exists in the database or false if it doesn't
                            return Convert.ToInt32(command.ExecuteScalar()) > 0;
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to check if the league name already exists."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Adding a new league to the database.
        /// </summary>
        /// <param name="league">The league object to be added.</param>
        /// <returns>True if the league has been added to the database or an exception if it couldn't be added.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to add the league to the database.</exception>
        public bool AddToDatabase(League league)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string insertQuery = "INSERT INTO Leagues (LeagueName) VALUES (@LeagueName); SELECT LAST_INSERT_ID();";

                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@LeagueName", league.Name);
                            league.LeagueID = Convert.ToInt32(command.ExecuteScalar());     // Retrieve the LeagueID from the database and assign it to the league object.

                            // Return true if the league has been added to the database or throw an exception if it couldn't be added
                            return league.LeagueID > 0 ? true : throw new Exception("Failed to add the league to the database.");
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to add the league to the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Retrieves all league records from the database.
        /// </summary>
        /// <returns>An ObservableCollection of League instances populated with data from the database.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="Exception">A league from the database has an invalid ID.</exception>
        /// <exception cref="MySqlException">Failed to get all the leagues from the database.</exception>
        public ObservableCollection<League> GetAllLeaguesFromDatabase()
        {
            ObservableCollection<League> leagues = new ObservableCollection<League>();

            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string selectQuery = "SELECT * FROM Leagues ORDER BY LeagueName ASC;";

                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())       // Iterate over the result set and populate the League objects.
                                {
                                    League league = new League(
                                        Convert.ToInt32(reader["LeagueID"]),
                                        reader["LeagueName"].ToString()
                                    );

                                    if (league.LeagueID > 0) { leagues.Add(league); }
                                    else { throw new Exception("A league from the database has an invalid ID."); }
                                }
                                reader.Close();
                                return leagues;
                            }
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to get all the leagues from the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }
    }
}
