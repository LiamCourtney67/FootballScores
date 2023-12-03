using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FootballScoresUI.models
{
    public class PlayerService
    {
        private readonly PlayerDataAccess _playerDataAccess;

        public PlayerService()
        {
            _playerDataAccess = new PlayerDataAccess();
        }

        public Player CreatePlayer(string firstName, string lastName, Team team)
        {
            Player newPlayer = new Player(firstName, lastName, team);

            if (_playerDataAccess.AddToDatabase(newPlayer))
            {
                return newPlayer;
            }
            else
            {
                throw new Exception("Failed to add player to the database.");
            }
        }

        public Player CreatePlayer(string firstName, string lastName, int age, int kitNumber, string position, Team team)
        {
            Player newPlayer = new Player(firstName, lastName, age, kitNumber, position, team);

            if (_playerDataAccess.AddToDatabase(newPlayer))
            {
                return newPlayer;
            }
            else
            {
                throw new Exception("Failed to add player to the database.");
            }
        }

        public ObservableCollection<Player> GetAllPlayersForTeam(Team team)
        {
            return _playerDataAccess.GetAllPlayersForTeamFromDatabase(team);
        }

        public void AddStatisticToDatabase(Player player, String stat)
        {
            PropertyInfo statPropertyInfo = typeof(Player).GetProperty(stat);
            _playerDataAccess.AddStatisticToDatabase(player, statPropertyInfo);
        }
    }
}
