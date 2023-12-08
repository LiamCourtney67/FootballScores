using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

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
                value = Regex.Replace(value, @"\s+", " "); // Replaces multiple spaces with a single space

                if (value.Length >= 3 && value.Length <= 35)
                {
                    if (value.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '\'' || char.IsWhiteSpace(c))) { _name = value.Trim(); }
                    else { throw new Exception("Name is not valid: can only contain letters, digits, - or '"); }
                }
                else { throw new Exception("Name is not valid: must be between 3 and 35 characters long."); }
            }
        }
        public ObservableCollection<Team> Teams { get => _teams; set => _teams = value; }
        public ObservableCollection<Match> Matches { get => matches; set => matches = value; }

        public LeagueService LeagueService { get => _leagueService; }

        /// <summary>
        /// Creating an instance of the <see cref="League"/> class.
        /// </summary>
        /// <param name="name">League name.</param>
        public League(string name) { this.Name = name; }

        /// <summary>
        /// Creating an instance of the <see cref="League"/> class with an existing league from the database.
        /// </summary>
        /// <param name="leagueID">LeagueID from the database.</param>
        /// <param name="name">League name from the database.</param>
        public League(int leagueID, string name)
        {
            this.LeagueID = leagueID;
            this.Name = name;
            this.Teams = LeagueService.TeamService.GetAllTeamsForLeague(this);          // Data coming from database is already sorted.
            this.Matches = LeagueService.MatchService.GetAllMatchesForLeague(this);     // Data coming from database is already sorted.
        }

        /// <summary>
        /// Adds a team to the ObservableCollection of teams and sorts the teams.
        /// </summary>
        /// <param name="team">Team object to be added.</param>
        public void AddTeam(Team team) { Teams.Add(team); SortTeams(); }

        /// <summary>
        /// Removes a team from the ObservableCollection of teams and sorts the teams.
        /// </summary>
        /// <param name="team">Team object to be removed.</param>
        public void RemoveTeam(Team team) { Teams.Remove(team); SortTeams(); }      // For version 2.0

        /// <summary>
        /// Adds a match to the ObservableCollection of matches and sorts the teams/matches.
        /// </summary>
        /// <param name="match">Match object to be added.</param>
        public void AddMatch(Match match) { Matches.Add(match); SortMatches(); SortTeams(); }

        /// <summary>
        /// Removes a match from the ObservableCollection of matches and sorts the teams/matches.
        /// </summary>
        /// <param name="match">Match object to be removed.</param>
        public void RemoveMatch(Match match) { Matches.Remove(match); SortMatches(); SortTeams(); }         // For version 2.0

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
