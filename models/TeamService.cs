using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Linking the TeamDataAccess and Team classes together.
    /// </summary>
    public class TeamService
    {
        private readonly TeamDataAccess _teamDataAccess;
        private readonly PlayerService _playerService;

        public PlayerService PlayerService { get => _playerService; }

        /// <summary>
        /// Instantiating the TeamService class.
        /// </summary>
        public TeamService()
        {
            _teamDataAccess = new TeamDataAccess();
            _playerService = new PlayerService();
        }

        /// <summary>
        /// Create a new team and add it to the database.
        /// </summary>
        /// <param name="name">Name of team to be added,</param>
        /// <param name="league">League object to be added.</param>
        /// <returns>Team object if added to the database successfuly or an exception if not.</returns>
        /// <exception cref="Exception">Team name already exists in the database.</exception>
        public Team CreateTeam(string name, League league)
        {
            if (_teamDataAccess.DoesTeamNameExistInLeague(name, league.LeagueID)) { throw new Exception("Could not add team: team name already exists in the database."); }
            else
            {
                try
                {
                    Team newTeam = new Team(name, league);
                    _teamDataAccess.AddToDatabase(newTeam);
                    return newTeam;
                }
                catch (Exception) { throw; }
            }
        }

        /// <summary>
        /// Get all teams from the database.
        /// </summary>
        /// <param name="league">League object to get the teams for.</param>
        /// <returns>An ObservableCollection of Team instances populated with data from the database.</returns>
        public ObservableCollection<Team> GetAllTeamsForLeague(League league)
        {
            try
            {
                ObservableCollection<Team> teams = _teamDataAccess.GetAllTeamsForLeagueFromDatabase(league);
                foreach (Team team in teams) { team.Players = PlayerService.GetAllPlayersForTeam(team); }
                return teams;
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Add a statistic to the database.
        /// </summary>
        /// <param name="team">Team object for adding statistics to.</param>
        /// <param name="stat">One of the following stats, 
        /// "GamesPlayed", "GamesWon", "GamesDrawn", "GamesLost", "GoalsFor", "GoalsAgainst", "GoalDifference", or "Points"</param>
        public void AddStatisticToDatabase(Team team, String stat)
        {
            try
            {
                PropertyInfo statPropertyInfo = typeof(Team).GetProperty(stat);
                _teamDataAccess.AddStatisticToDatabase(team, statPropertyInfo);
            }
            catch (Exception) { throw; }
        }
    }
}
