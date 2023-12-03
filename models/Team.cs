using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        private TeamService _teamService = new TeamService();   // This is used to pull players and matches from the database and push new stats.

        public int TeamID { get => _teamID; set => _teamID = value; }
        public string Name
        {
            get => _name;
            private set
            {
                if (value.Length >= 3 || value.Length <= 35)
                {
                    if (!value.Any(c => (char.IsPunctuation(c) || char.IsSymbol(c)) && c != '-' && c != '\''))
                    {
                        _name = value.Trim();
                    }
                    else
                    {
                        throw new Exception("Name is not valid: cannot contain punctuation except - or '");
                    }
                }
                else
                {
                    throw new Exception("Name is not valid: must be between 3 and 35 characters long.");
                }
            }
        }
        public League League { get => _league; private set => _league = value; }
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

        public Team(string name, League league)
        {
            this.Name = name;
            this.League = league;
            league.AddTeam(this);
        }

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

        public void AddPlayer(Player player) { Players.Add(player); SortPlayers(); }

        public void RemovePlayer(Player player) { Players.Remove(player); SortPlayers(); }

        private void SortPlayers() { Players = new ObservableCollection<Player>(Players.OrderBy(player => player.KitNumber)); }

        public void AddMatch(Match match) { Matches.Add(match); SortMatches(); }

        public void RemoveMatch(Match match) { Matches.Remove(match); SortMatches(); }

        public void SortMatches() { Matches = new ObservableCollection<Match>(Matches.OrderBy(match => match.DatePlayed)); }

    }
}
