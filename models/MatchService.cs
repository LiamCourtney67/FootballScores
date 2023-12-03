using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI.models
{
    public class MatchService
    {
        private readonly MatchDataAccess _matchDataAccess;
        private readonly TeamService _teamService;
        private readonly PlayerService _playerService;

        public TeamService TeamService { get => _teamService; }
        public PlayerService PlayerService { get => _playerService; }

        public MatchService()
        {
            _matchDataAccess = new MatchDataAccess();

            _teamService = new TeamService();
            _playerService = new PlayerService();
        }

        public Match CreateMatch(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals)
        {
            Match newMatch = new Match(homeTeam, awayTeam, datePlayed, homeGoals, awayGoals);
            AssignTeamStatsToDatabase(newMatch);
            AssignPlayerStatsToDatabase(newMatch);

            if (_matchDataAccess.AddToDatabase(newMatch))
            {
                return newMatch;
            }
            else
            {
                throw new Exception("Failed to add match to the database.");
            }
        }

        public Match CreateMatch(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals,
            ObservableCollection<Player> homeScorers, ObservableCollection<Player> homeAssists, ObservableCollection<Player> awayScorers,
            ObservableCollection<Player> awayAssists, ObservableCollection<Player> yellowCards, ObservableCollection<Player> redCards)
        {
            Match newMatch = new Match(homeTeam, awayTeam, datePlayed, homeGoals, awayGoals, homeScorers, homeAssists, awayScorers, awayAssists, yellowCards, redCards);
            AssignTeamStatsToDatabase(newMatch);
            AssignPlayerStatsToDatabase(newMatch);

            if (_matchDataAccess.AddToDatabase(newMatch))
            {
                return newMatch;
            }
            else
            {
                throw new Exception("Failed to add match to the database.");
            }
        }

        public ObservableCollection<Match> GetAllMatchesForLeague(League league)
        {
            return _matchDataAccess.GetAllMatchesForLeagueFromDatabase(league);
        }

        public void AssignTeamStatsToDatabase(Match match)
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
