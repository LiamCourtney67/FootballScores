using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FootballScoresUI.models
{
    public class MatchDataAccess
    {
        /// <summary>
        /// Checks if a match with the same teams and date played already exists within the database
        /// </summary>
        /// <param name="homeTeam">The home team to be checked</param>
        /// <param name="awayTeam">The away team to be checked</param>
        /// <param name="datePlayed">The date played to be checked</param>
        /// <returns>True if a match already exists within the database or false if it doesn't</returns>
        /// <exception cref="Exception"></exception>
        private bool DoesMatchExist(Team homeTeam, Team awayTeam, DateTime datePlayed)
        {
            try
            {
                // Attempt to open the database connection
                DatabaseConnection.OpenConnection();

                using (MySqlConnection connection = DatabaseConnection.GetConnection())
                {
                    // SQL query to check if a team with the same name exists in the league
                    string checkQuery = "SELECT COUNT(*) FROM Matches WHERE HomeTeamID = @HomeTeamID AND AwayTeamID = @AwayTeamID AND DatePlayed = @DatePlayed;";

                    using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                    {
                        // Add the teamIDs and date played as a parameter to the SQL query
                        command.Parameters.AddWithValue("@HomeTeamID", homeTeam.TeamID);
                        command.Parameters.AddWithValue("@AwayTeamID", awayTeam.TeamID);
                        command.Parameters.AddWithValue("@DatePlayed", datePlayed.ToString("yyyy-MM-dd"));

                        try
                        {
                            // Execute the SQL query and get the count
                            int count = Convert.ToInt32(command.ExecuteScalar());

                            return count > 0;
                        }
                        catch (MySqlException e)
                        {
                            throw new Exception("Failed to check if the match already exists within the database: " + e.Message + " " + e.InnerException);
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
        /// Adding a new match to the database
        /// </summary>
        /// <param name="match">The match object to be added to the database</param>
        /// <returns>True if the match has been added to the database or an exception if it couldn't be added</returns>
        /// <exception cref="Exception"></exception>
        public bool AddToDatabase(Match match)
        {
            if (DoesMatchExist(match.HomeTeam, match.AwayTeam, match.DatePlayed))
            {
                throw new Exception("A match with the same teams and date played already exists in the database.");
            }

            MySqlConnection connection = null;
            MySqlTransaction transaction = null;    // Use transaction to ensure all operations are executed or none are.

            try
            {
                DatabaseConnection.OpenConnection();
                connection = DatabaseConnection.GetConnection();
                transaction = connection.BeginTransaction();

                // SQL query to insert a new match and get its ID
                string insertMatchQuery = "INSERT INTO Matches (HomeTeamID, AwayTeamID, LeagueID, DatePlayed, HomeGoals, AwayGoals, Result) VALUES (@HomeTeamID, @AwayTeamID, @LeagueID, @DatePlayed, @HomeGoals, @AwayGoals, @Result); SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(insertMatchQuery, connection, transaction))
                {
                    // Add match parameters
                    command.Parameters.AddWithValue("@HomeTeamID", match.HomeTeam.TeamID);
                    command.Parameters.AddWithValue("@AwayTeamID", match.AwayTeam.TeamID);
                    command.Parameters.AddWithValue("@LeagueID", match.HomeTeam.League.LeagueID);
                    command.Parameters.AddWithValue("@DatePlayed", match.DatePlayed.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@HomeGoals", match.HomeGoals);
                    command.Parameters.AddWithValue("@AwayGoals", match.AwayGoals);
                    command.Parameters.AddWithValue("@Result", match.Result);

                    match.MatchID = Convert.ToInt32(command.ExecuteScalar());
                }

                // Check if match ID is valid
                if (match.MatchID <= 0)
                {
                    throw new Exception("Failed to add the match to the database.");
                }

                // SQL query to insert match details
                string insertMatchDetailsQuery = "INSERT INTO MatchStatsDetails (MatchID, PlayerID, Goals, Assists, YellowCards, RedCards) VALUES (@MatchID, @PlayerID, @Goals, @Assists, @YellowCards, @RedCards);";
                using (MySqlCommand command = new MySqlCommand(insertMatchDetailsQuery, connection, transaction))
                {
                    // Prepare command parameters that don't change per insert
                    command.Parameters.Add("@MatchID", MySqlDbType.Int32).Value = match.MatchID;
                    var playerIdParam = command.Parameters.Add("@PlayerID", MySqlDbType.Int32);
                    var goalsParam = command.Parameters.Add("@Goals", MySqlDbType.Int32);
                    var assistsParam = command.Parameters.Add("@Assists", MySqlDbType.Int32);
                    var yellowCardsParam = command.Parameters.Add("@YellowCards", MySqlDbType.Int32);
                    var redCardsParam = command.Parameters.Add("@RedCards", MySqlDbType.Int32);

                    var uniqueHomePlayers = match.HomeScorers
                        .Concat(match.HomeAssists)
                        .Concat(match.HomeYellowCards)
                        .Concat(match.HomeRedCards)
                        .GroupBy(p => p.PlayerID)
                        .Select(g => g.First());

                    var uniqueAwayPlayers = match.AwayScorers
                        .Concat(match.AwayAssists)
                        .Concat(match.AwayYellowCards)
                        .Concat(match.AwayRedCards)
                        .GroupBy(p => p.PlayerID)
                        .Select(g => g.First());

                    // Insert match details for each player involved
                    foreach (Player player in uniqueHomePlayers)
                    {
                        // Set parameters for each player
                        playerIdParam.Value = player.PlayerID;
                        goalsParam.Value = match.HomeScorers.Count(p => p.PlayerID == player.PlayerID);
                        assistsParam.Value = match.HomeAssists.Count(p => p.PlayerID == player.PlayerID);
                        yellowCardsParam.Value = match.HomeYellowCards.Count(p => p.PlayerID == player.PlayerID);
                        redCardsParam.Value = match.HomeRedCards.Count(p => p.PlayerID == player.PlayerID);

                        command.ExecuteNonQuery();
                    }

                    foreach (Player player in uniqueAwayPlayers)
                    {
                        // Set parameters for each player
                        playerIdParam.Value = player.PlayerID;
                        goalsParam.Value = match.AwayScorers.Count(p => p.PlayerID == player.PlayerID);
                        assistsParam.Value = match.AwayAssists.Count(p => p.PlayerID == player.PlayerID);
                        yellowCardsParam.Value = match.AwayYellowCards.Count(p => p.PlayerID == player.PlayerID);
                        redCardsParam.Value = match.AwayRedCards.Count(p => p.PlayerID == player.PlayerID);

                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (MySqlException e)
            {
                transaction?.Rollback();
                throw new Exception("Failed to execute database operations: " + e.Message + " " + e.InnerException);
            }
            catch (Exception e)
            {
                transaction?.Rollback();
                throw new Exception("Failed to interact with the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Close the database connection
                DatabaseConnection.CloseConnection();
            }
        }

        /// <summary>
        /// Retrieves all match records for a league from the database.
        /// </summary>
        /// <param name="league">The league to retrieve the matches for.</param>
        /// <returns>A ObservableCollection of match instances from a league populated with data from the database.</returns>
        public ObservableCollection<Match> GetAllMatchesForLeagueFromDatabase(League league)
        {
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
                    // Define a SQL query to retrieve all match records for a league.
                    string selectQuery = @"
                        SELECT m.*, p.*, msd.* 
                        FROM Matches m
                        LEFT JOIN MatchStatsDetails msd ON m.MatchID = msd.MatchID
                        LEFT JOIN Players p ON msd.PlayerID = p.PlayerID
                        WHERE m.LeagueID = @LeagueID 
                        ORDER BY m.DatePlayed ASC, msd.PlayerID ASC;";

                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        // Add the league id as a parameter to the SQL query.
                        command.Parameters.AddWithValue("@LeagueID", league.LeagueID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {

                            // Iterate over the result set and populate the Match objects.
                            Dictionary<int, Match> matchDictionary = new Dictionary<int, Match>();

                            while (reader.Read())
                            {
                                int matchID = Convert.ToInt32(reader["MatchID"]);
                                Match match;

                                if (!matchDictionary.TryGetValue(matchID, out match))
                                {
                                    // If this is a new match we haven't seen yet, create it and add to dictionary
                                    Team homeTeam = league.Teams.FirstOrDefault(team => team.TeamID == Convert.ToInt32(reader["HomeTeamID"]));
                                    Team awayTeam = league.Teams.FirstOrDefault(team => team.TeamID == Convert.ToInt32(reader["AwayTeamID"]));

                                    match = new Match(
                                            matchID,
                                            homeTeam,
                                            awayTeam,
                                            Convert.ToDateTime(reader["DatePlayed"]),
                                            Convert.ToInt32(reader["HomeGoals"]),
                                            Convert.ToInt32(reader["AwayGoals"]),
                                            reader["Result"].ToString(),
                                            new ObservableCollection<Player>(),     // Home Scorers
                                            new ObservableCollection<Player>(),     // Home Assists
                                            new ObservableCollection<Player>(),     // Away Scorers
                                            new ObservableCollection<Player>(),     // Away Assists
                                            new ObservableCollection<Player>(),     // YellowCards
                                            new ObservableCollection<Player>()      // RedCards
                                        );

                                    matchDictionary.Add(matchID, match);
                                }

                                // Add player statistics to the match
                                int playerID = Convert.ToInt32(reader["PlayerID"]);
                                Player player = match.HomeTeam.Players.Concat(match.AwayTeam.Players).FirstOrDefault(p => p.PlayerID == playerID);
                                if (player != null && player.Team.TeamID == match.HomeTeam.TeamID)
                                {
                                    for (int i = 0; i < Convert.ToInt32(reader["Goals"]); i++) match.HomeScorers.Add(player);
                                    for (int i = 0; i < Convert.ToInt32(reader["Assists"]); i++) match.HomeAssists.Add(player);
                                    for (int i = 0; i < Convert.ToInt32(reader["YellowCards"]); i++) match.HomeYellowCards.Add(player);
                                    for (int i = 0; i < Convert.ToInt32(reader["RedCards"]); i++) match.HomeRedCards.Add(player);
                                }
                                else if (player != null && player.Team.TeamID == match.AwayTeam.TeamID)
                                {
                                    for (int i = 0; i < Convert.ToInt32(reader["Goals"]); i++) match.AwayScorers.Add(player);
                                    for (int i = 0; i < Convert.ToInt32(reader["Assists"]); i++) match.AwayAssists.Add(player);
                                    for (int i = 0; i < Convert.ToInt32(reader["YellowCards"]); i++) match.AwayYellowCards.Add(player);
                                    for (int i = 0; i < Convert.ToInt32(reader["RedCards"]); i++) match.AwayRedCards.Add(player);
                                }
                            }

                            reader.Close();

                            // Convert the dictionary values to an ObservableCollection
                            ObservableCollection<Match> matches = new ObservableCollection<Match>(matchDictionary.Values);
                            return matches;
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                // Handle any MySQL-specific exceptions that might arise during database operations.
                throw new Exception("An error occurred while retrieving matches from the database: " + e.Message + " " + e.InnerException);
            }
            finally
            {
                // Ensure the database connection is closed after the operation.
                DatabaseConnection.CloseConnection();
            }
        }
    }
}
