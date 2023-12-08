using MySqlConnector;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Database access class for the Team.
    /// </summary>
    public class TeamDataAccess
    {
        /// <summary>
        /// Checks if a team with the same name already exists in the league within the database.
        /// </summary>
        /// <param name="teamName">The team name to be checked.</param>
        /// <param name="leagueID">The ID of the league to be checked against.</param>
        /// <returns>True if a team already exists with that name in the league within the database or false if it doesn't.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to check if the team name already exists in the league.</exception>
        public bool DoesTeamNameExistInLeague(string teamName, int leagueID)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Teams WHERE TeamName = @TeamName AND LeagueID = @LeagueID;";

                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@TeamName", teamName);
                            command.Parameters.AddWithValue("@LeagueID", leagueID);

                            return Convert.ToInt32(command.ExecuteScalar()) > 0;
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to check if the team name already exists in the league."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Adding a new team to the database.
        /// </summary>
        /// <param name="team">The team object to be added to the database.</param>
        /// <returns>True if the team has been added to the database or an exception if it couldn't be added.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to add the team to the database.</exception>"
        public bool AddToDatabase(Team team)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string insertQuery = "INSERT INTO Teams (TeamName, LeagueID, GamesPlayed, GamesWon, GamesDrawn, GamesLost, GoalsFor, GoalsAgainst, GoalDifference, Points) " +
                            "VALUES (@TeamName, @LeagueID, @GamesPlayed, @GamesWon, @GamesDrawn, @GamesLost, @GoalsFor, @GoalsAgainst, @GoalDifference, @Points); SELECT LAST_INSERT_ID();";
                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                        {
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

                            team.TeamID = Convert.ToInt32(command.ExecuteScalar());

                            // Return true if the team has been added to the database or throw an exception if it couldn't be added
                            return team.TeamID > 0 ? true : throw new Exception("Failed to add the team to the database.");
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to add the league to the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Retrieves all team records for a league from the database.
        /// </summary>
        /// <param name="league">The league to retrieve the teams for.</param>
        /// <returns>An ObservableCollection of team instances from a league populated with data from the database.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="Exception">A team from the database has an invalid ID.</exception>
        /// <exception cref="MySqlException">Failed to get all the teams from the database.</exception>
        public ObservableCollection<Team> GetAllTeamsForLeagueFromDatabase(League league)
        {
            // Initialize a ObservableCollection to store and return the Team objects.
            ObservableCollection<Team> teams = new ObservableCollection<Team>();

            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string selectQuery = @" SELECT * FROM Teams WHERE LeagueID = @LeagueID
                            ORDER BY Points DESC, GoalDifference DESC, GoalsFor DESC, TeamName ASC;";

                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
                            command.Parameters.AddWithValue("@LeagueID", league.LeagueID);

                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
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

                                    if (team.TeamID > 0) { teams.Add(team); }
                                    else { throw new Exception("A team retrieved from the database has an invalid ID."); }
                                }
                                reader.Close();
                                return teams;
                            }
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to get all the teams from the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Add a statistic to the database for a specific team.
        /// </summary>
        /// <param name="team">The team for the stats to be added to.</param>
        /// <param name="property">The property of the stat to be added.</param>
        /// <exception cref="ArgumentException">Invalid stat property name.</exception>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to add the stat to the database.</exception>
        public void AddStatisticToDatabase(Team team, PropertyInfo property)
        {
            // Validate that the property name matches a valid column name to prevent SQL injection.
            string[] validStats = new string[] { "GamesPlayed", "GamesWon", "GamesDrawn", "GamesLost", "GoalsFor", "GoalsAgainst", "GoalDifference", "Points" };
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
                            string updateQuery = $"UPDATE Teams SET {stat} = @StatValue WHERE TeamID = @TeamID;";

                            using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@StatValue", property.GetValue(team));
                                command.Parameters.AddWithValue("@TeamID", team.TeamID);
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
