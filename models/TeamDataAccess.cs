using MySqlConnector;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace FootballScoresUI.models
{
    public class TeamDataAccess
    {
        /// <summary>
        /// Checks if a team with the same name already exists in the league within the database
        /// </summary>
        /// <param name="teamName">The team name to be checked</param>
        /// <param name="leagueID">The ID of the league to be checked against</param>
        /// <returns>True if a team already exists with that name in the league within the database or false if it doesn't</returns>
        /// <exception cref="Exception"></exception>
        private bool DoesTeamNameExistInLeague(string teamName, int leagueID)
        {
            try
            {
                // Attempt to open the database connection
                DatabaseConnection.OpenConnection();

                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    // SQL query to check if a team with the same name exists in the league
                    string checkQuery = "SELECT COUNT(*) FROM Teams WHERE TeamName = @TeamName AND LeagueID = @LeagueID;";

                    using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                    {
                        // Add the team name and leagueID as a parameter to the SQL query
                        command.Parameters.AddWithValue("@TeamName", teamName);
                        command.Parameters.AddWithValue("@LeagueID", leagueID);

                        try
                        {
                            // Execute the SQL query and get the count
                            int count = Convert.ToInt32(command.ExecuteScalar());

                            return count > 0;
                        }
                        catch (MySqlException e)
                        {
                            throw new Exception("Failed to check if the team name already exists in the league within the database: " + e.Message + " " + e.InnerException);
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
        /// Adding a new team to the database
        /// </summary>
        /// <param name="team">The team object to be added to the database</param>
        /// <returns>True if the team has been added to the database or an exception if it couldn't be added</returns>
        /// <exception cref="Exception"></exception>
        public bool AddToDatabase(Team team)
        {
            // Check if a team with the same name already exists in the league
            if (DoesTeamNameExistInLeague(team.Name, team.League.LeagueID))
            {
                throw new Exception("A team with the same name already exists in the league.");
            }

            try
            {
                // Attempt to open the database connection
                DatabaseConnection.OpenConnection();

                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    // SQL query to insert a new team and get its ID
                    string insertQuery = "INSERT INTO Teams (TeamName, LeagueID, GamesPlayed, GamesWon, GamesDrawn, GamesLost, GoalsFor, GoalsAgainst, GoalDifference, Points) VALUES (@TeamName, @LeagueID, @GamesPlayed, @GamesWon, @GamesDrawn, @GamesLost, @GoalsFor, @GoalsAgainst, @GoalDifference, @Points); SELECT LAST_INSERT_ID();";
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        // Add the team parameters to the SQL query
                        command.Parameters.AddWithValue("@TeamName", team.Name);
                        command.Parameters.AddWithValue("@LeagueID", team.League.LeagueID);
                        command.Parameters.AddWithValue("@GamesPlayed", team.GamesPlayed);
                        command.Parameters.AddWithValue("@GamesWon", team.GamesWon);
                        command.Parameters.AddWithValue("@GamesDrawn", team.GamesDrawn);
                        command.Parameters.AddWithValue("@GamesLost", team.GamesLost);
                        command.Parameters.AddWithValue("@GoalsFor", team.GoalsFor);
                        command.Parameters.AddWithValue("@GoalsAgainst", team.GoalsAgainst);
                        command.Parameters.AddWithValue("@GoalDifference", team.GoalDifference);
                        command.Parameters.AddWithValue("@Points", team.Points);

                        // Execute the SQL query and get the new league's ID
                        team.TeamID = Convert.ToInt32(command.ExecuteScalar());

                        // Return true if the team has been added to the database or throw an exception if it couldn't be added
                        return team.TeamID > 0 ? true : throw new Exception("Failed to add the team to the database.");
                    }
                }
            }
            catch (MySqlException e)
            {
                throw new Exception("Failed to execute the insert query or retrieve the TeamID: " + e.Message + " " + e.InnerException);
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
        /// Retrieves all team records for a league from the database.
        /// </summary>
        /// <param name="league">The league to retrieve the teams for.</param>
        /// <returns>A ObservableCollection of team instances from a league populated with data from the database.</returns>
        public ObservableCollection<Team> GetAllTeamsForLeagueFromDatabase(League league)
        {
            // Initialize a ObservableCollection to store and return the Team objects.
            ObservableCollection<Team> teams = new ObservableCollection<Team>();

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
                    // Define a SQL query to retrieve all team records for a league.
                    string selectQuery = @"
                            SELECT * 
                            FROM Teams 
                            WHERE LeagueID = @LeagueID
                            ORDER BY Points DESC, GoalDifference DESC, GoalsFor DESC, TeamName ASC;
                        ";

                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        // Add the league id as a parameter to the SQL query.
                        command.Parameters.AddWithValue("@LeagueID", league.LeagueID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {

                            // Iterate over the result set and populate the Team objects.
                            while (reader.Read())
                            {
                                Team team = new Team(
                                Convert.ToInt32(reader["TeamID"]),
                                reader["TeamName"].ToString(),
                                league,
                                Convert.ToInt32(reader["GamesPlayed"]),
                                Convert.ToInt32(reader["GamesWon"]),
                                Convert.ToInt32(reader["GamesDrawn"]),
                                Convert.ToInt32(reader["GamesLost"]),
                                Convert.ToInt32(reader["GoalsFor"]),
                                Convert.ToInt32(reader["GoalsAgainst"]),
                                Convert.ToInt32(reader["GoalDifference"]),
                                Convert.ToInt32(reader["Points"])
                                );

                                // Validate the team object (i.e., ensure it has a valid ID) and add it to the ObservableCollection.
                                if (team.TeamID > 0)
                                {
                                    teams.Add(team);
                                }
                                else
                                {
                                    throw new Exception("A team retrieved from the database has an invalid ID.");
                                }
                            }
                            reader.Close();
                            return teams;
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle any MySql-specific exceptions that might arise during database operations.
                throw new Exception("An error occurred while retrieving teams from the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Ensure the database connection is closed after the operation.
                DatabaseConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Add a statistic to the database for a specific team.
        /// </summary>
        /// <param name="team">The team for the stats to be added to.</param>
        /// <param name="property">The property of the stat to be added.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public void AddStatisticToDatabase(Team team, PropertyInfo property)
        {
            // Validate that the property name matches a valid column name to prevent SQL injection.
            string[] validStats = new string[] { "GamesPlayed", "GamesWon", "GamesDrawn", "GamesLost", "GoalsFor", "GoalsAgainst", "GoalDifference", "Points" };
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
                    // Define a SQL query to update a specific team record.
                    string updateQuery = $"UPDATE Teams SET {stat} = @StatValue WHERE TeamID = @TeamID;";

                    using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                    {
                        // Add the stat value and teamID as parameters to the SQL query.
                        command.Parameters.AddWithValue("@StatValue", property.GetValue(team));
                        command.Parameters.AddWithValue("@TeamID", team.TeamID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle any MySql-specific exceptions that might arise during database operations.
                throw new Exception("An error occurred while updating team stats to the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Ensure the database connection is closed after the operation.
                DatabaseConnection.CloseConnection();
            }
        }
    }
}
