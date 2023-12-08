using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Team object storing the team name/ID, league, players, matches, and statistics.
    /// </summary>
    public class Team
    {
        private int _teamID;        // This is set by the database.
        private string _name;
        private League _league;
        private ObservableCollection<Player> _players = new ObservableCollection<Player>();
        private ObservableCollection<Match> matches = new ObservableCollection<Match>();

        // Statistics.
        private int _gamesPlayed;
        private int _gamesWon;
        private int _gamesDrawn;
        private int _gamesLost;
        private int _goalsFor;
        private int _goalsAgainst;
        private int _goalDifference;
        private int _points;

        private TeamService _teamService = new TeamService();

        public int TeamID { get => _teamID; set => _teamID = value; }
        public string Name
        {
            get => _name;
            private set
            {
                value = Regex.Replace(value, @"\s+", " "); // Replaces multiple spaces with a single space

                if (value.Length >= 3 && value.Length <= 35)
                {
                    if (value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '\'' || char.IsWhiteSpace(c))) { _name = value.Trim(); }
                    else { throw new Exception("Name is not valid: can only contain letters, digits, - or '"); }
                }
                else { throw new Exception("Name is not valid: must be between 3 and 35 characters long."); }
            }
        }
        public League League
        {
            get => _league;
            private set
            {
                if (value != null) { _league = value; }
                else { throw new Exception("League is not valid: cannot be empty."); }
            }
        }
        public ObservableCollection<Player> Players { get => _players; set => _players = value; }
        public ObservableCollection<Match> Matches { get => matches; set => matches = value; }

        // Statistics.
        public int GamesPlayed { get => _gamesPlayed; private set => _gamesPlayed = value; }
        public int GamesWon { get => _gamesWon; private set => _gamesWon = value; }
        public int GamesDrawn { get => _gamesDrawn; private set => _gamesDrawn = value; }
        public int GamesLost { get => _gamesLost; private set => _gamesLost = value; }
        public int GoalsFor { get => _goalsFor; private set => _goalsFor = value; }
        public int GoalsAgainst { get => _goalsAgainst; private set => _goalsAgainst = value; }
        public int GoalDifference { get => _goalDifference; private set => _goalDifference = value; }
        public int Points { get => _points; private set => _points = value; }

        public TeamService TeamService { get => _teamService; }

        /// <summary>
        /// Creating an instance of the <see cref="Team"/> class.
        /// </summary>
        /// <param name="name">The name of the team to be added.</param>
        /// <param name="league">The league object for the team.</param>
        public Team(string name, League league)
        {
            this.Name = name;
            this.League = league;
            league.AddTeam(this);
        }

        /// <summary>
        /// Creating an instance of the <see cref="Team"/> class with an existing team from the database.
        /// </summary>
        /// <param name="teamID">Team ID from the database.</param>
        /// <param name="name">Team name from the database.</param>
        /// <param name="league">League object of the team from the database.</param>
        /// <param name="gamesPlayed">Amount of games played from the database.</param>
        /// <param name="gamesWon">Amount of games won from the database.</param>
        /// <param name="gamesDrawn">Amount of games drawn from the database.</param>
        /// <param name="gamesLost">Amount of games lost from the database.</param>
        /// <param name="goalsFor">Amount of goals scored from the database.</param>
        /// <param name="goalsAgainst">Amount of goals conceded from the database.</param>
        /// <param name="goalDifference">Amount of goals scored minus goals against from the database.</param>
        /// <param name="points">Amount of points from the database.</param>
        public Team(int teamID, string name, League league, int gamesPlayed, int gamesWon, int gamesDrawn, int gamesLost, int goalsFor, int goalsAgainst, int goalDifference, int points)
        {
            this.TeamID = teamID;
            this.League = league;
            this.Name = name;
            this.Players = TeamService.PlayerService.GetAllPlayersForTeam(this);
            SortPlayers();
            this.GamesPlayed = gamesPlayed;
            this.GamesWon = gamesWon;
            this.GamesDrawn = gamesDrawn;
            this.GamesLost = gamesLost;
            this.GoalsFor = goalsFor;
            this.GoalsAgainst = goalsAgainst;
            this.GoalDifference = goalDifference;
            this.Points = points;
        }

        /// <summary>
        /// Assign team statistics based on the goals scored and conceded.
        /// </summary>
        /// <param name="goalsFor">Amount of goals scored in a match.</param>
        /// <param name="goalsAgainst">Amount of goals conceded in a match.</param>
        public void AssignTeamStats(int goalsFor, int goalsAgainst)
        {
            GamesPlayed++;
            GoalsFor += goalsFor;
            GoalsAgainst += goalsAgainst;

            if (goalsFor > goalsAgainst)
            {
                GamesWon++;
                _teamService.AddStatisticToDatabase(this, "GamesWon");
            }
            else if (goalsFor < goalsAgainst)
            {
                GamesLost++;
                _teamService.AddStatisticToDatabase(this, "GamesLost");
            }
            else if (goalsFor == goalsAgainst)
            {
                GamesDrawn++;
                _teamService.AddStatisticToDatabase(this, "GamesDrawn");
            }

            Points = (GamesWon * 3) + GamesDrawn;
            GoalDifference = GoalsFor - GoalsAgainst;
        }

        /// <summary>
        /// Adds a player to the ObservableCollection of players and sorts the players.
        /// </summary>
        /// <param name="player">Player object to be added.</param>
        public void AddPlayer(Player player) { Players.Add(player); SortPlayers(); }

        /// <summary>
        /// Removes a player from the ObservableCollection of players and sorts the players.
        /// </summary>
        /// <param name="player">Player object to be removed.</param>
        public void RemovePlayer(Player player) { Players.Remove(player); SortPlayers(); }

        /// <summary>
        /// Sorts the ObservableCollection of players in the team by kit number.
        /// </summary>
        private void SortPlayers() { Players = new ObservableCollection<Player>(Players.OrderBy(player => player.KitNumber)); }

        /// <summary>
        /// Adds a match to the ObservableCollection of matches and sorts the matches.
        /// </summary>
        /// <param name="match">Match object to be added.</param>
        public void AddMatch(Match match) { Matches.Add(match); SortMatches(); }

        /// <summary>
        /// Removes a match from the ObservableCollection of matches and sorts the matches.
        /// </summary>
        /// <param name="match">Match object to be removed.</param>
        public void RemoveMatch(Match match) { Matches.Remove(match); SortMatches(); }

        /// <summary>
        /// Sorts the ObservableCollection of matches in the team by date played.
        /// </summary>
        public void SortMatches() { Matches = new ObservableCollection<Match>(Matches.OrderBy(match => match.DatePlayed)); }

    }
}
