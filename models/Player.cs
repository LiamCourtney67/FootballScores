using System;
using System.Linq;

namespace FootballScoresUI.models
{
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
                if (value.Length >= 3 || value.Length <= 35)
                {
                    if (!value.Any(c => (char.IsPunctuation(c) || char.IsSymbol(c)) && c != '-' && c != '\''))
                    {
                        if (!value.Any(c => char.IsDigit(c)))
                        {
                            _firstName = value.Trim();
                        }
                        else
                        {
                            throw new Exception("First name is not valid: cannot contain a digit.");
                        }
                    }
                    else
                    {
                        throw new Exception("First name is not valid: cannot contain punctuation except - or '");
                    }
                }
                else
                {
                    throw new Exception("First name is not valid: must be between 3 and 35 characters long.");
                }
            }
        }
        public string LastName
        {
            get => _lastName;
            private set
            {
                if (value.Length >= 3 || value.Length <= 35)
                {
                    if (!value.Any(c => (char.IsPunctuation(c) || char.IsSymbol(c)) && c != '-' && c != '\''))
                    {
                        if (!value.Any(c => char.IsDigit(c)))
                        {
                            _lastName = value.Trim();
                        }
                        else
                        {
                            throw new Exception("Last name is not valid: cannot contain a digit.");
                        }
                    }
                    else
                    {
                        throw new Exception("Last name is not valid: cannot contain punctuation except - or '");
                    }
                }
                else
                {
                    throw new Exception("Last name is not valid: must be between 3 and 35 characters long.");
                }
            }
        }
        public string Name { get => $"{FirstName} {LastName}"; }
        public int Age
        {
            get => _age;
            private set
            {
                if (value > 0 || value < 100)
                {
                    _age = value;
                }
                else
                {
                    throw new Exception("Age is not valid: must be a whole number between 1 and 99.");
                }
            }
        }
        public int KitNumber
        {
            get => _kitNumber;
            private set
            {
                if (value > 0 || value < 100)
                {
                    _kitNumber = value;
                }
                else
                {
                    throw new Exception("Kit Number is not valid: must be a whole number between 1 and 99.");
                }
            }
        }
        public string Position
        {
            get => _position;
            private set
            {
                if (value == "Goalkeeper" || value == "Defender" || value == "Midfielder" || value == "Attacker")
                {
                    _position = value;
                }
                else
                {
                    throw new Exception("Position is not valid: must be Goalkeeper, Defender, Midfielder, or Attacker.");
                }
            }
        }
        public Team Team
        {
            get => _team;
            private set => _team = value;
        }
        public int GoalsScored { get => _goalsScored; private set => _goalsScored = value; }
        public int Assists { get => _assists; private set => _assists = value; }
        public int CleanSheets { get => _cleanSheets; private set => _cleanSheets = value; }
        public int YellowCards { get => _yellowCards; private set => _yellowCards = value; }
        public int RedCards { get => _redCards; private set => _redCards = value; }

        public Player(string firstName, string lastName, Team team)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Team = team;
            team.AddPlayer(this);
        }

        public Player(string firstName, string lastName, int age, int kitNumber, string position, Team team)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.KitNumber = kitNumber;
            this.Position = position;
            this.Team = team;
            team.AddPlayer(this);
        }

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

        public void ScoreGoal(int amount) { GoalsScored += amount; }

        public void AssistGoal(int amount) { Assists += amount; }

        public void CleanSheet() { CleanSheets++; }

        public void YellowCard(int amount) { YellowCards += amount; }

        public void RedCard() { RedCards++; }

    }
}
