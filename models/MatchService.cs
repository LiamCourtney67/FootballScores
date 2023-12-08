using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Linking the MatchDataAccess and Match classes together.
    /// </summary>
    public class MatchService
    {
        private readonly MatchDataAccess _matchDataAccess;
        private readonly TeamService _teamService;
        private readonly PlayerService _playerService;

        public TeamService TeamService { get => _teamService; }
        public PlayerService PlayerService { get => _playerService; }

        /// <summary>
        /// Initialises a new instance of the <see cref="MatchService"/> class.
        /// </summary>
        public MatchService()
        {
            _matchDataAccess = new MatchDataAccess();

            _teamService = new TeamService();
            _playerService = new PlayerService();
        }

        /// <summary>
        /// Creates a new match and adds it to the database.
        /// </summary>
        /// <param name="homeTeam">Team object of home team.</param>
        /// <param name="awayTeam">Team object of away team.</param>
        /// <param name="datePlayed">DateTime of date played.</param>
        /// <param name="homeGoals">Amount of goals scored by the home team.</param>
        /// <param name="awayGoals">Amount of goals scored by the away team.</param>
        /// <returns></League object if added to the database successfuly or an exception if not.returns>
        /// <exception cref="Exception">Match already exists in the database.</exception>
        public Match CreateMatch(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals)
        {
            if (_matchDataAccess.DoesMatchExist(homeTeam, awayTeam, datePlayed)) { throw new Exception("Could not add match: match already exists in the database."); }
            else
            {
                try
                {
                    Match newMatch = new Match(homeTeam, awayTeam, datePlayed, homeGoals, awayGoals);
                    AssignTeamStatsToDatabase(newMatch);
                    AssignPlayerStatsToDatabase(newMatch);
                    _matchDataAccess.AddToDatabase(newMatch);
                    return newMatch;
                }
                catch (Exception) { throw; }
            }
        }

        /// <summary>
        /// Creates a new match and adds it to the database.
        /// </summary>
        /// <param name="homeTeam">Team object of home team.</param>
        /// <param name="awayTeam">Team object of away team.</param>
        /// <param name="datePlayed">DateTime of date played.</param>
        /// <param name="homeGoals">Amount of goals scored by the home team.</param>
        /// <param name="awayGoals">Amount of goals scored by the away team.</param>
        /// <param name="homeScorers">Observable Collection of home players that scored.</param>
        /// <param name="homeAssists">Observable Collection of home players that assisted.</param>
        /// <param name="awayScorers">Observable Collection of away players that scored.</param>
        /// <param name="awayAssists">Observable Collection of away players that assisted.</param>
        /// <param name="yellowCards">Observable Collection of players that got a yellow card.</param>
        /// <param name="redCards">Observable Collection of players that got a red card.</param>
        /// <returns></League object if added to the database successfuly or an exception if not.returns>
        /// <exception cref="Exception">Match already exists in the database.</exception>
        public Match CreateMatch(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals,
            ObservableCollection<Player> homeScorers, ObservableCollection<Player> homeAssists, ObservableCollection<Player> awayScorers,
            ObservableCollection<Player> awayAssists, ObservableCollection<Player> yellowCards, ObservableCollection<Player> redCards)
        {
            if (_matchDataAccess.DoesMatchExist(homeTeam, awayTeam, datePlayed)) { throw new Exception("Could not add match: match already exists in the database."); }
            else
            {
                try
                {
                    Match newMatch = new Match(homeTeam, awayTeam, datePlayed, homeGoals, awayGoals, homeScorers, homeAssists, awayScorers, awayAssists, yellowCards, redCards);
                    AssignTeamStatsToDatabase(newMatch);
                    AssignPlayerStatsToDatabase(newMatch);
                    _matchDataAccess.AddToDatabase(newMatch);
                    return newMatch;
                }
                catch (Exception) { throw; }
            }
        }

        /// <summary>
        /// Get all matches from the database.
        /// </summary>
        /// <param name="league">League object to get the matches for.</param>
        /// <returns>An ObservableCollection of Match instances populated with data from the database.</returns>
        public ObservableCollection<Match> GetAllMatchesForLeague(League league)
        {
            try { return _matchDataAccess.GetAllMatchesForLeagueFromDatabase(league); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Assign team statistics to the database.
        /// </summary>
        /// <param name="match">Match object to get the team statistics from.</param>
        public void AssignTeamStatsToDatabase(Match match)
        {
            try
            {
                TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesPlayed");
                TeamService.AddStatisticToDatabase(match.HomeTeam, "GoalsFor");
                TeamService.AddStatisticToDatabase(match.HomeTeam, "GoalsAgainst");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesPlayed");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "GoalsFor");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "GoalsAgainst");

                if (match.Result == "Home")
                {
                    TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesWon");
                    TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesLost");
                }
                else if (match.Result == "Draw")
                {
                    TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesDrawn");
                    TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesDrawn");
                }
                else if (match.Result == "Away")
                {
                    TeamService.AddStatisticToDatabase(match.HomeTeam, "GamesLost");
                    TeamService.AddStatisticToDatabase(match.AwayTeam, "GamesWon");
                }

                TeamService.AddStatisticToDatabase(match.HomeTeam, "Points");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "Points");
                TeamService.AddStatisticToDatabase(match.HomeTeam, "GoalDifference");
                TeamService.AddStatisticToDatabase(match.AwayTeam, "GoalDifference");
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Assign player statistics to the database.
        /// </summary>
        /// <param name="match">Match object to get the player statistics from.</param>
        public void AssignPlayerStatsToDatabase(Match match)
        {
            foreach (Player player in match.HomeScorers) { PlayerService.AddStatisticToDatabase(player, "GoalsScored"); }
            foreach (Player player in match.HomeAssists) { PlayerService.AddStatisticToDatabase(player, "Assists"); }
            foreach (Player player in match.HomeYellowCards) { PlayerService.AddStatisticToDatabase(player, "YellowCards"); }
            foreach (Player player in match.HomeRedCards) { PlayerService.AddStatisticToDatabase(player, "RedCards"); }

            foreach (Player player in match.AwayScorers) { PlayerService.AddStatisticToDatabase(player, "GoalsScored"); }
            foreach (Player player in match.AwayAssists) { PlayerService.AddStatisticToDatabase(player, "Assists"); }
            foreach (Player player in match.AwayYellowCards) { PlayerService.AddStatisticToDatabase(player, "YellowCards"); }
            foreach (Player player in match.AwayRedCards) { PlayerService.AddStatisticToDatabase(player, "RedCards"); }

            foreach (Player player in match.CleanSheets) { PlayerService.AddStatisticToDatabase(player, "CleanSheets"); }
        }
    }
}
