using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Linking the PlayerDataAccess and Player classes together.
    /// </summary>
    public class PlayerService
    {
        private readonly PlayerDataAccess _playerDataAccess;

        /// <summary>
        /// Instantiating the PlayerService class.
        /// </summary>
        public PlayerService() { _playerDataAccess = new PlayerDataAccess(); }

        /// <summary>
        /// Create a new player and add it to the database.
        /// </summary>
        /// <param name="firstName">First name of the player.</param>
        /// <param name="lastName">Last name of the player.</param>
        /// <param name="age">Age of the player.</param>
        /// <param name="kitNumber">Kit number of the player.</param>
        /// <param name="position">Position of the player.</param>
        /// <param name="team">Team object for the player.</param>
        /// <returns>Player object if added to the database successfuly or an exception if not.</returns>
        /// <exception cref="Exception">Kit number already exists in the team within database.</exception>
        public Player CreatePlayer(string firstName, string lastName, int age, int kitNumber, string position, Team team)
        {
            if (_playerDataAccess.DoesKitNumberExistInTeam(kitNumber, team.TeamID)) { throw new Exception("Could not add player: kit number already exists in the database."); }
            else
            {
                Player newPlayer = new Player(firstName, lastName, age, kitNumber, position, team);
                _playerDataAccess.AddToDatabase(newPlayer);
                return newPlayer;
            }
        }

        /// <summary>
        /// Get all players from the database.
        /// </summary>
        /// <param name="team">Team object to get players from.</param>
        /// <returns>An ObservableCollection of player instances populated with data from the database.</returns>
        public ObservableCollection<Player> GetAllPlayersForTeam(Team team)
        {
            try { return _playerDataAccess.GetAllPlayersForTeamFromDatabase(team); }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Add a statistic to the database.
        /// </summary>
        /// <param name="player">Player object for stats to be added to.</param>
        /// <param name="stat">One of the following stats, "GoalsScored", "Assists", "CleanSheets", "YellowCards", or "RedCards"</param>
        public void AddStatisticToDatabase(Player player, String stat)
        {
            try
            {
                PropertyInfo statPropertyInfo = typeof(Player).GetProperty(stat);
                _playerDataAccess.AddStatisticToDatabase(player, statPropertyInfo);
            }
            catch (Exception) { throw; }
        }
    }
}
