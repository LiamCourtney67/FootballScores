using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI.models
{
    public class LeagueService
    {
        private readonly LeagueDataAccess _leagueDataAccess;
        private readonly TeamService _teamService;
        private readonly MatchService _matchService;

        public TeamService TeamService { get => _teamService; }
        public MatchService MatchService { get => _matchService; }

        public LeagueService()
        {
            _leagueDataAccess = new LeagueDataAccess();
            _teamService = new TeamService();
            _matchService = new MatchService();
        }

        public League CreateLeague(string name)
        {
            League newLeague = new League(name);

            if (_leagueDataAccess.AddToDatabase(newLeague))
            {
                return newLeague;
            }
            else
            {
                throw new Exception("Failed to add league to the database.");
            }
        }

        public ObservableCollection<League> GetAllLeagues()
        {
            ObservableCollection<League> leagues = _leagueDataAccess.GetAllLeaguesFromDatabase();
            foreach (League league in leagues)
            {
                league.Teams = _teamService.GetAllTeamsForLeague(league);
                league.SortTeams();
                league.Matches = MatchService.GetAllMatchesForLeague(league);
                league.SortMatches();
            }
            return leagues;
        }
    }

}
