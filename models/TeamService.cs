using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FootballScoresUI.models
{
    public class TeamService
    {
        private readonly TeamDataAccess _teamDataAccess;
        private readonly PlayerService _playerService;

        public PlayerService PlayerService { get => _playerService; }

        public TeamService()
        {
            _teamDataAccess = new TeamDataAccess();

            _playerService = new PlayerService();
        }

        public Team CreateTeam(string name, League league)
        {
            Team newTeam = new Team(name, league);

            if (_teamDataAccess.AddToDatabase(newTeam))
            {
                return newTeam;
            }
            else
            {
                throw new Exception("Failed to add team to the database.");
            }
        }

        public ObservableCollection<Team> GetAllTeamsForLeague(League league)
        {
            ObservableCollection<Team> teams = _teamDataAccess.GetAllTeamsForLeagueFromDatabase(league);
            foreach (Team team in teams)
            {
                team.Players = PlayerService.GetAllPlayersForTeam(team);
            }
            return teams;
        }

        public void AddStatisticToDatabase(Team team, String stat)
        {
            PropertyInfo statPropertyInfo = typeof(Team).GetProperty(stat);
            _teamDataAccess.AddStatisticToDatabase(team, statPropertyInfo);
        }
    }
}
