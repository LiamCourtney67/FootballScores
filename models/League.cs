using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FootballScoresUI.models
{
    /// <summary>
    /// League object storing the league name/ID, teams, and matches.
    /// </summary>
    public class League
    {
        private int _leagueID;      // This is set by the database.
        private string _name = "";
        private ObservableCollection<Team> _teams = new ObservableCollection<Team>();
        private ObservableCollection<Match> matches = new ObservableCollection<Match>();

        private LeagueService _leagueService = new LeagueService();

        public int LeagueID { get => _leagueID; set => _leagueID = value; }
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
        public ObservableCollection<Team> Teams { get => _teams; set => _teams = value; }
        public ObservableCollection<Match> Matches { get => matches; set => matches = value; }

        public LeagueService LeagueService { get => _leagueService; }

        /// <summary>
        /// Creating an instance of the League class.
        /// </summary>
        /// <param name="name">League name.</param>
        public League(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Creating an instance of the League class with an existing league from the database.
        /// </summary>
        /// <param name="leagueID">LeagueID from the database.</param>
        /// <param name="name">League name from the database.</param>
        public League(int leagueID, string name)
        {
            this.LeagueID = leagueID;
            this.Name = name;
            this.Teams = LeagueService.TeamService.GetAllTeamsForLeague(this);          // Data coming from database is alreacy sorted.
            this.Matches = LeagueService.MatchService.GetAllMatchesForLeague(this);     // Data coming from database is alreacy sorted.
        }

        // Adding/removing/sorting teams and matches.

        /// <summary>
        /// Adds a team to the ObservableCollection of teams.
        /// </summary>
        /// <param name="team">Team object to be added.</param>
        public void AddTeam(Team team) { Teams.Add(team); SortTeams(); }

        /// <summary>
        /// Removes a team from the ObservableCollection of teams.
        /// </summary>
        /// <param name="team">Team object to be removed.</param>
        public void RemoveTeam(Team team) { Teams.Remove(team); SortTeams(); }

        /// <summary>
        /// Adds a match to the ObservableCollection of matches.
        /// </summary>
        /// <param name="team">Match object to be added.</param>
        public void AddMatch(Match match) { Matches.Add(match); SortMatches(); SortTeams(); }

        /// <summary>
        /// Removes a match from the ObservableCollection of matches.
        /// </summary>
        /// <param name="team">Match object to be removed.</param>
        public void RemoveMatch(Match match) { Matches.Remove(match); SortMatches(); SortTeams(); }

        /// <summary>
        /// Sorts the ObservableCollection of teams in the league by points, goal difference, goals for, and name.
        /// </summary>
        public void SortTeams()
        {
            var sortedTeams = Teams
                .OrderByDescending(team => team.Points)
                .ThenByDescending(team => team.GoalDifference)
                .ThenByDescending(team => team.GoalsFor)
                .ThenBy(team => team.Name)
                .ToList();

            Teams = new ObservableCollection<Team>(sortedTeams);
        }

        /// <summary>
        /// Sorts the ObservableCollection of matches in the league by date played.
        /// </summary>
        public void SortMatches()
        {
            var sortedMatches = Matches
                .OrderBy(match => match.DatePlayed)
                .ToList();

            Matches = new ObservableCollection<Match>(sortedMatches);
        }
    }
}
