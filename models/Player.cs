using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Player object storing the player name/ID, age, kit number, position, team, and statistics.
    /// </summary>
    public class Player
    {
        private int _playerID;      // This is set by the database.
        private string _firstName;
        private string _lastName;
        private int _age;
        private int _kitNumber;
        private string _position;
        private Team _team;

        // Statistics.
        private int _goalsScored = 0;
        private int _assists = 0;
        private int _cleanSheets = 0;
        private int _yellowCards = 0;
        private int _redCards = 0;

        public int PlayerID { get => _playerID; set => _playerID = value; }
        public string FirstName
        {
            get => _firstName;
            private set
            {
                value = Regex.Replace(value, @"\s+", " "); // Replaces multiple spaces with a single space

                if (value.Length >= 2 && value.Length <= 35)
                {
                    if (value.All(c => char.IsLetter(c) || c == '-' || c == '\'' || char.IsWhiteSpace(c))) { _firstName = value.Trim(); }
                    else { throw new Exception("First name is not valid: can only contain letters, -, or '"); }
                }
                else { throw new Exception("First name is not valid: must be between 2 and 35 characters long."); }
            }
        }
        public string LastName
        {
            get => _lastName;
            private set
            {
                value = Regex.Replace(value, @"\s+", " "); // Replaces multiple spaces with a single space

                if (value.Length >= 2 && value.Length <= 35)
                {
                    if (value.All(c => char.IsLetter(c) || c == '-' || c == '\'' || char.IsWhiteSpace(c))) { _lastName = value.Trim(); }
                    else { throw new Exception("Last name is not valid: can only contain letters, -, or '"); }
                }
                else { throw new Exception("Last name is not valid: must be between 2 and 35 characters long."); }
            }
        }
        public string Name { get => $"{FirstName} {LastName}"; }
        public int Age
        {
            get => _age;
            private set
            {
                if (value > 0 && value < 100) { _age = value; }
                else { throw new Exception("Age is not valid: must be a whole number between 1 and 99."); }
            }
        }
        public int KitNumber
        {
            get => _kitNumber;
            private set
            {
                if (value > 0 && value < 100) { _kitNumber = value; }
                else { throw new Exception("Kit Number is not valid: must be a whole number between 1 and 99."); }
            }
        }
        public string Position
        {
            get => _position;
            private set
            {
                if (value == "Goalkeeper" || value == "Defender" || value == "Midfielder" || value == "Attacker") { _position = value; }
                else { throw new Exception("Position is not valid: must be Goalkeeper, Defender, Midfielder, or Attacker."); }
            }
        }
        public Team Team
        {
            get => _team;
            private set
            {
                if (value != null) { _team = value; }
                else { throw new Exception("Team is not valid: cannot be empty."); }
            }
        }
        public int GoalsScored { get => _goalsScored; private set => _goalsScored = value; }
        public int Assists { get => _assists; private set => _assists = value; }
        public int CleanSheets { get => _cleanSheets; private set => _cleanSheets = value; }
        public int YellowCards { get => _yellowCards; private set => _yellowCards = value; }
        public int RedCards { get => _redCards; private set => _redCards = value; }

        /// <summary>
        /// Creating an instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="firstName">First name of the player</param>
        /// <param name="lastName">Last name of the player</param>
        /// <param name="age">Age of the player</param>
        /// <param name="kitNumber">Kit number of the player</param>
        /// <param name="position">Position of the player</param>
        /// <param name="team">Team object of the player</param>
        public Player(string firstName, string lastName, int age, int kitNumber, string position, Team team)
        {
            this.Team = team;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.Position = position;
            this.KitNumber = kitNumber;
            team.AddPlayer(this);
        }

        /// <summary>
        /// Creating an instance of the <see cref="Player"/> class with an existing player from the database.
        /// </summary>
        /// <param name="playerID">Player ID from the database.</param>
        /// <param name="firstName">First name from the database.</param>
        /// <param name="lastName">Last name from the database.</param>
        /// <param name="age">Age from the database.</param>
        /// <param name="kitNumber">Kit number from the database.</param>
        /// <param name="position">Position from the database.</param>
        /// <param name="team">Team object of the player from the database.</param>
        /// <param name="goalsScored">Amount of goals scored from the database.</param>
        /// <param name="assists">Amount of assists from the database.</param>
        /// <param name="cleanSheets">Amount of clean sheets from the database.</param>
        /// <param name="yellowCards">Amount of yellow cards from the database.</param>
        /// <param name="redCards">Amount of red cards from the database.</param>
        public Player(int playerID, string firstName, string lastName, int age, int kitNumber, string position, Team team, int goalsScored, int assists, int cleanSheets, int yellowCards, int redCards)
        {
            this.PlayerID = playerID;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.KitNumber = kitNumber;
            this.Position = position;
            this.Team = team;
            this.GoalsScored = goalsScored;
            this.Assists = assists;
            this.CleanSheets = cleanSheets;
            this.YellowCards = yellowCards;
            this.RedCards = redCards;
        }

        /// <summary>
        /// Add goals for the player.
        /// </summary>
        /// <param name="amount">Amount of goals scored.</param>
        public void ScoreGoal(int amount) { GoalsScored += amount; }

        /// <summary>
        /// Add assists for the player.
        /// </summary>
        /// <param name="amount">Amount of assists.</param>
        public void AssistGoal(int amount) { Assists += amount; }

        /// <summary>
        /// Add a clean sheet for the player.
        /// </summary>
        public void CleanSheet() { CleanSheets++; }

        /// <summary>
        /// Add yellow cards for the player.
        /// </summary>
        /// <param name="amount">Amount of yellow cards.</param>
        public void YellowCard(int amount) { YellowCards += amount; }

        /// <summary>
        /// Add a red card for the player.
        /// </summary>
        public void RedCard() { RedCards++; }
    }
}
