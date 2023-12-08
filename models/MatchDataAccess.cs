using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Database access class for the Match.
    /// </summary>
    public class MatchDataAccess
    {
        /// <summary>
        /// Checks if a match with the same teams and date played already exists within the database.
        /// </summary>
        /// <param name="homeTeam">The home team to be checked.</param>
        /// <param name="awayTeam">The away team to be checked.</param>
        /// <param name="datePlayed">The date played to be checked.</param>
        /// <returns>True if a match already exists within the database or false if it doesn't.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Falied to check if the team name already exists in the league.</exception>
        public bool DoesMatchExist(Team homeTeam, Team awayTeam, DateTime datePlayed)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        string checkQuery = "SELECT COUNT(*) FROM Matches WHERE HomeTeamID = @HomeTeamID AND AwayTeamID = @AwayTeamID AND DatePlayed = @DatePlayed;";

                        using (MySqlCommand command = new MySqlCommand(checkQuery, connection))
                        {
                            command.Parameters.AddWithValue("@HomeTeamID", homeTeam.TeamID);
                            command.Parameters.AddWithValue("@AwayTeamID", awayTeam.TeamID);
                            command.Parameters.AddWithValue("@DatePlayed", datePlayed.ToString("yyyy-MM-dd"));

                            // Return true if the team name already exists in the league within the database or false if it doesn't
                            return Convert.ToInt32(command.ExecuteScalar()) > 0;
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to check if the match already exists in the league."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Adding a new match to the database.
        /// </summary>
        /// <param name="match">The match object to be added to the database.</param>
        /// <returns>True if the match has been added to the database or an exception if it couldn't be added.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="Exception">Failed to add the match to the database.</exception>
        /// <exception cref="MySqlException">Failed to add the match to the database.</exception>"
        public bool AddToDatabase(Match match)
        {
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;    // Use transaction to ensure all operations are executed or none are.

            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (connection = DatabaseConnection.GetConnection())
                    {
                        transaction = connection.BeginTransaction();

                        // SQL query to insert a new match and get its ID
                        string insertQuery = "INSERT INTO Matches (HomeTeamID, AwayTeamID, LeagueID, DatePlayed, HomeGoals, AwayGoals, Result) " +
                            "VALUES (@HomeTeamID, @AwayTeamID, @LeagueID, @DatePlayed, @HomeGoals, @AwayGoals, @Result); SELECT LAST_INSERT_ID();";

                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@HomeTeamID", match.HomeTeam.TeamID);
                            command.Parameters.AddWithValue("@AwayTeamID", match.AwayTeam.TeamID);
                            command.Parameters.AddWithValue("@LeagueID", match.HomeTeam.League.LeagueID);
                            command.Parameters.AddWithValue("@DatePlayed", match.DatePlayed.ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@HomeGoals", match.HomeGoals);
                            command.Parameters.AddWithValue("@AwayGoals", match.AwayGoals);
                            command.Parameters.AddWithValue("@Result", match.Result);

                            match.MatchID = Convert.ToInt32(command.ExecuteScalar());

                            if (match.MatchID <= 0) { throw new Exception("Failed to add the match to the database."); }
                        }

                        // SQL query to insert match details
                        insertQuery = "INSERT INTO MatchStatsDetails (MatchID, PlayerID, MatchGoals, MatchAssists, MatchYellowCards, MatchRedCards) " +
                            "VALUES (@MatchID, @PlayerID, @MatchGoals, @MatchAssists, @MatchYellowCards, @MatchRedCards);";
                        using (MySqlCommand command = new MySqlCommand(insertQuery, connection, transaction))
                        {
                            command.Parameters.Add("@MatchID", MySqlDbType.Int32).Value = match.MatchID;
                            var playerIdParam = command.Parameters.Add("@PlayerID", MySqlDbType.Int32);
                            var goalsParam = command.Parameters.Add("@MatchGoals", MySqlDbType.Int32);
                            var assistsParam = command.Parameters.Add("@MatchAssists", MySqlDbType.Int32);
                            var yellowCardsParam = command.Parameters.Add("@MatchYellowCards", MySqlDbType.Int32);
                            var redCardsParam = command.Parameters.Add("@MatchRedCards", MySqlDbType.Int32);

                            // Get unique players from each collection
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
                                playerIdParam.Value = player.PlayerID;
                                goalsParam.Value = match.HomeScorers.Count(p => p.PlayerID == player.PlayerID);
                                assistsParam.Value = match.HomeAssists.Count(p => p.PlayerID == player.PlayerID);
                                yellowCardsParam.Value = match.HomeYellowCards.Count(p => p.PlayerID == player.PlayerID);
                                redCardsParam.Value = match.HomeRedCards.Count(p => p.PlayerID == player.PlayerID);

                                command.ExecuteNonQuery();
                            }

                            foreach (Player player in uniqueAwayPlayers)
                            {
                                playerIdParam.Value = player.PlayerID;
                                goalsParam.Value = match.AwayScorers.Count(p => p.PlayerID == player.PlayerID);
                                assistsParam.Value = match.AwayAssists.Count(p => p.PlayerID == player.PlayerID);
                                yellowCardsParam.Value = match.AwayYellowCards.Count(p => p.PlayerID == player.PlayerID);
                                redCardsParam.Value = match.AwayRedCards.Count(p => p.PlayerID == player.PlayerID);

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            return match.MatchID > 0;
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to add the match to the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }

        /// <summary>
        /// Retrieves all match records for a league from the database.
        /// </summary>
        /// <param name="league">The league to retrieve the matches for.</param>
        /// <returns>An ObservableCollection of match instances from a league populated with data from the database.</returns>
        /// <exception cref="Exception">Database isn't able to open a connection.</exception>
        /// <exception cref="MySqlException">Failed to get all the matches from the database.</exception>
        public ObservableCollection<Match> GetAllMatchesForLeagueFromDatabase(League league)
        {
            if (!DatabaseConnection.OpenConnection()) { throw new Exception("Failed to open the database connection."); }
            else
            {
                try
                {
                    using (MySqlConnection connection = DatabaseConnection.GetConnection())
                    {
                        // Define a SQL query to retrieve all match records for a league.
                        string selectQuery = @"SELECT m.*, p.*, msd.* 
                        FROM Matches m
                        LEFT JOIN MatchStatsDetails msd ON m.MatchID = msd.MatchID
                        LEFT JOIN Players p ON msd.PlayerID = p.PlayerID
                        WHERE m.LeagueID = @LeagueID 
                        ORDER BY m.DatePlayed ASC, msd.PlayerID ASC;";

                        using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                        {
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
                                    if ((reader["PlayerID"] != DBNull.Value) && (match.MatchID == Convert.ToInt32(reader["MatchID"])))
                                    {
                                        int playerID = Convert.ToInt32(reader["PlayerID"]);
                                        Player player = match.HomeTeam.Players.Concat(match.AwayTeam.Players).FirstOrDefault(p => p.PlayerID == playerID);
                                        if (player != null && player.Team.TeamID == match.HomeTeam.TeamID)
                                        {
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchGoals"]); i++) match.HomeScorers.Add(player);
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchAssists"]); i++) match.HomeAssists.Add(player);
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchYellowCards"]); i++) match.HomeYellowCards.Add(player);
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchRedCards"]); i++) match.HomeRedCards.Add(player);
                                        }
                                        else if (player != null && player.Team.TeamID == match.AwayTeam.TeamID)
                                        {
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchGoals"]); i++) match.AwayScorers.Add(player);
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchAssists"]); i++) match.AwayAssists.Add(player);
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchYellowCards"]); i++) match.AwayYellowCards.Add(player);
                                            for (int i = 0; i < Convert.ToInt32(reader["MatchRedCards"]); i++) match.AwayRedCards.Add(player);
                                        }
                                    }
                                }

                                reader.Close();

                                ObservableCollection<Match> matches = new ObservableCollection<Match>(matchDictionary.Values);
                                return matches;
                            }
                        }
                    }
                }
                catch (MySqlException) { throw new Exception("Failed to get all the matches from the database."); }
                finally { DatabaseConnection.CloseConnection(); }
            }
        }
    }
}
