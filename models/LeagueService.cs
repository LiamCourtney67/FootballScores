using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Linking the LeagueDataAccess and League classes together.
    /// </summary>
    public class LeagueService
    {
        private readonly LeagueDataAccess _leagueDataAccess;
        private readonly TeamService _teamService;
        private readonly MatchService _matchService;

        public TeamService TeamService { get => _teamService; }
        public MatchService MatchService { get => _matchService; }

        /// <summary>
        /// Creating an instance of the LeagueService class.
        /// </summary>
        public LeagueService()
        {
            _leagueDataAccess = new LeagueDataAccess();
            _teamService = new TeamService();
            _matchService = new MatchService();
        }

        /// <summary>
        /// Create a new league and add it to the database.
        /// </summary>
        /// <param name="name">Name of league to be added.</param>
        /// <returns>League object if added to the database successfuly or an exception if not.</returns>
        /// <exception cref="Exception">League name already exists in the database.</exception>
        public League CreateLeague(string name)
        {
            if (_leagueDataAccess.DoesLeagueNameExist(name)) { throw new Exception("Could not add league: league name already exists in the database."); }
            else
            {
                try
                {
                    League newLeague = new League(name);
                    _leagueDataAccess.AddToDatabase(newLeague);
                    return newLeague;
                }
                catch (Exception) { throw; }
            }
        }

        /// <summary>
        /// Get all leagues from the database.
        /// </summary>
        /// <returns>An ObservableCollection of League instances populated with data from the database.</returns>
        public ObservableCollection<League> GetAllLeagues()
        {
            ObservableCollection<League> leagues = _leagueDataAccess.GetAllLeaguesFromDatabase();

            try
            {
                foreach (League league in leagues)
                {
                    league.Teams = _teamService.GetAllTeamsForLeague(league);
                    league.SortTeams();
                    league.Matches = MatchService.GetAllMatchesForLeague(league);
                    league.SortMatches();
                }
                return leagues;
            }
            catch (Exception) { throw; }
        }
    }
}
