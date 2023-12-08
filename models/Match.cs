using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI.models
{
    /// <summary>
    /// Match object storing the home/away teams, date played, and goals scored.
    /// </summary>
    public class Match
    {
        private int _matchID;       // This is set by the database.
        private Team _homeTeam;
        private Team _awayTeam;
        private League _league;
        private DateTime _datePlayed;
        private int _homeGoals;
        private int _awayGoals;
        private string _result;

        // Player statistics.
        private ObservableCollection<Player> _homeScorers = new ObservableCollection<Player>();
        private ObservableCollection<Player> _homeAssists = new ObservableCollection<Player>();
        private ObservableCollection<Player> _homeYellowCards = new ObservableCollection<Player>();
        private ObservableCollection<Player> _homeRedCards = new ObservableCollection<Player>();

        private ObservableCollection<Player> _awayScorers = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayAssists = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayYellowCards = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayRedCards = new ObservableCollection<Player>();

        private ObservableCollection<Player> _cleanSheets = new ObservableCollection<Player>();

        public int MatchID { get => _matchID; set => _matchID = value; }
        public Team HomeTeam
        {
            get => _homeTeam;
            private set
            {
                if (value != null)
                {
                    if (value != AwayTeam) { _homeTeam = value; }
                    else { throw new Exception("Home team not valid: teams cannot be the same."); }
                }
                else { throw new Exception("Home team not valid: home team cannot be empty."); }
            }
        }
        public Team AwayTeam
        {
            get => _awayTeam;
            private set
            {
                if (value != null)
                {
                    if (value != HomeTeam) { _awayTeam = value; }
                    else { throw new Exception("Away team not valid: teams cannot be the same."); }
                }
                else { throw new Exception("Away team not valid: away team cannot be empty."); }
            }
        }
        public DateTime DatePlayed
        {
            get => _datePlayed;
            private set
            {
                if (value <= DateTime.Now) { _datePlayed = value; }
                else { throw new Exception("Date Played not valid: date cannot be in the future."); }
            }
        }
        public League League
        {
            get => _league;
            private set
            {
                if (value != null) { _league = value; }
                else { throw new Exception("League not valid: league cannot be empty."); }
            }
        }
        public int HomeGoals
        {
            get => _homeGoals;
            private set
            {
                if (value >= 0 || value <= 99) { _homeGoals = value; }
                else { throw new Exception("Home goals not valid: goals must be between 0 and 99."); }
            }
        }
        public int AwayGoals
        {
            get => _awayGoals;
            private set
            {
                if (value >= 0 || value <= 99) { _awayGoals = value; }
                else { throw new Exception("Away goals not valid: goals must be between 0 and 99."); }
            }
        }
        public string Result { get => _result; private set => _result = value; }
        public string MatchData { get => $"{DatePlayed.ToString("dd-MM-yyyy")}: {HomeTeam.Name} {HomeGoals} - {AwayGoals} {AwayTeam.Name}"; }

        // Player statistics.
        public ObservableCollection<Player> HomeScorers { get => _homeScorers; private set => _homeScorers = value; }
        public ObservableCollection<Player> HomeAssists { get => _homeAssists; private set => _homeAssists = value; }
        public ObservableCollection<Player> HomeYellowCards { get => _homeYellowCards; private set => _homeYellowCards = value; }
        public ObservableCollection<Player> HomeRedCards { get => _homeRedCards; private set => _homeRedCards = value; }

        public ObservableCollection<Player> AwayScorers { get => _awayScorers; private set => _awayScorers = value; }
        public ObservableCollection<Player> AwayAssists { get => _awayAssists; private set => _awayAssists = value; }
        public ObservableCollection<Player> AwayYellowCards { get => _awayYellowCards; private set => _awayYellowCards = value; }
        public ObservableCollection<Player> AwayRedCards { get => _awayRedCards; private set => _awayRedCards = value; }

        public ObservableCollection<Player> CleanSheets { get => _cleanSheets; private set => _cleanSheets = value; }

        /// <summary>
        /// Creating an instance of the <see cref="Match"/> class.
        /// </summary>
        /// <param name="homeTeam">Team object for home team.</param>
        /// <param name="awayTeam">Team object for away team.</param>
        /// <param name="datePlayed">DateTime of the date the match was played.</param>
        /// <param name="homeGoals">Number of goals scored by the home team.</param>
        /// <param name="awayGoals">Number of goals scored by the away team.</param>
        /// <exception cref="Exception">Teams are the same or are not in the same league</exception>
        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals)
        {
            this.DatePlayed = datePlayed;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.League = homeTeam.League;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = CalculateResult();

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID && HomeTeam != AwayTeam)
            {
                this.HomeTeam.League.AddMatch(this);
                this.HomeTeam.AddMatch(this);
                this.AwayTeam.AddMatch(this);
                HomeTeam.AssignTeamStats(homeGoals, awayGoals);
                AwayTeam.AssignTeamStats(awayGoals, homeGoals);
                CheckCleenSheets();
            }
            else { throw new Exception("Teams not valid: teams are the same or are not in the same league."); }

        }

        /// <summary>
        /// Creating an instance of the <see cref="Match"/> class.
        /// </summary>
        /// <param name="homeTeam">Team object of home team.</param>
        /// <param name="awayTeam">Team object of away team.</param>
        /// <param name="datePlayed">DateTime for date played.</param>
        /// <param name="homeGoals">Amount of goals scored by the home team.</param>
        /// <param name="awayGoals">Amount of goals scored by the away team.</param>
        /// <param name="homeScorers">Observable Collection of home players that scored.</param>
        /// <param name="homeAssists">Observable Collection of home players that assisted.</param>
        /// <param name="awayScorers">Observable Collection of away players that scored.</param>
        /// <param name="awayAssists">Observable Collection of away players that assisted.</param>
        /// <param name="yellowCards">Observable Collection of players that got a yellow card.</param>
        /// <param name="redCards">Observable Collection of players that got a red card.</param>
        /// <exception cref="Exception">Teams are the same or are not in the same league</exception>
        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals,
            ObservableCollection<Player> homeScorers, ObservableCollection<Player> homeAssists, ObservableCollection<Player> awayScorers,
            ObservableCollection<Player> awayAssists, ObservableCollection<Player> yellowCards, ObservableCollection<Player> redCards)
        {
            this.DatePlayed = datePlayed;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.League = homeTeam.League;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = CalculateResult();

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID && HomeTeam != AwayTeam)
            {
                AddScorer(homeScorers);
                AddAssist(homeAssists);
                AddScorer(awayScorers);
                AddAssist(awayAssists);
                AddYellowCards(yellowCards);
                AddRedCards(redCards);
                this.HomeTeam.League.AddMatch(this);
                this.HomeTeam.AddMatch(this);
                this.AwayTeam.AddMatch(this);
                HomeTeam.AssignTeamStats(homeGoals, awayGoals);
                AwayTeam.AssignTeamStats(awayGoals, homeGoals);
                CheckCleenSheets();
            }
            else { throw new Exception("Teams not valid: teams are the same or are not in the same league."); }
        }

        /// <summary>
        /// Creating an instance of the <see cref="Match"/> class with an existing match from the database.
        /// </summary>
        /// <param name="matchID">MatchID from the database.</param>
        /// <param name="homeTeam">Team object of home team from the database.</param>
        /// <param name="awayTeam">Team object of away team from the database.</param>
        /// <param name="datePlayed">DateTime for date played from the database.</param>
        /// <param name="homeGoals">Amount of goals scored by the home team from the database.</param>
        /// <param name="awayGoals">Amount of goals scored by the away team from the database.</param>
        /// <param name="result">Result from the database.</param>
        /// <param name="homeScorers">Observable Collection of home players that scored from the database.</param>
        /// <param name="homeAssists">Observable Collection of home players that assisted from the database.</param>
        /// <param name="awayScorers">Observable Collection of away players that scored from the database.</param>
        /// <param name="awayAssists">Observable Collection of away players that assisted from the database.</param>
        /// <param name="yellowCards">Observable Collection of players that got a yellow card from the database.</param>
        /// <param name="redCards">Observable Collection of players that got a red card from the database.</param>
        public Match(int matchID, Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals, string result,
            ObservableCollection<Player> homeScorers, ObservableCollection<Player> homeAssists, ObservableCollection<Player> awayScorers,
            ObservableCollection<Player> awayAssists, ObservableCollection<Player> yellowCards, ObservableCollection<Player> redCards)
        {
            this.MatchID = matchID;
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.League = homeTeam.League;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = result;

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID && HomeTeam != AwayTeam)
            {
                AddScorer(homeScorers);
                AddAssist(homeAssists);
                AddScorer(awayScorers);
                AddAssist(awayAssists);
                AddYellowCards(yellowCards);
                AddRedCards(redCards);
                this.HomeTeam.League.AddMatch(this);
                this.HomeTeam.AddMatch(this);
                this.AwayTeam.AddMatch(this);
            }
            else { throw new Exception("Teams not valid: teams are the same or are not in the same league."); }
        }

        /// <summary>
        /// Calculates the result of the match.
        /// </summary>
        /// <returns>"Home", "Away", or "Draw" depending on the result.</returns>
        private string CalculateResult()
        {
            if (HomeGoals > AwayGoals) { return "Home"; }
            else if (AwayGoals > HomeGoals) { return "Away"; }
            else { return "Draw"; }
        }

        /// <summary>
        /// Checks if a player has a clean sheet and assigns one if they do.
        /// </summary>
        private void CheckCleenSheets()
        {
            if (HomeGoals == 0)
            {
                foreach (Player player in AwayTeam.Players)
                {
                    if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    {
                        CleanSheets.Add(player);
                        player.CleanSheet();
                    }
                }
            }
            if (AwayGoals == 0)
            {
                foreach (Player player in HomeTeam.Players)
                {
                    if (player.Position == "Goalkeeper" || player.Position == "Defender")
                    {
                        CleanSheets.Add(player);
                        player.CleanSheet();
                    }
                }
            }
        }

        /// <summary>
        /// Scoring a goal for a player.
        /// </summary>
        /// <param name="scorers">Observable Collection of players who scored a goal.</param>
        /// <exception cref="Exception">Player doesn't play for either team.</exception>
        private void AddScorer(ObservableCollection<Player> scorers)
        {
            foreach (Player player in scorers)
            {
                if (player.Team == HomeTeam)
                {
                    HomeScorers.Add(player);
                    player.ScoreGoal(1);
                }
                else if (player.Team == AwayTeam)
                {
                    AwayScorers.Add(player);
                    player.ScoreGoal(1);
                }
                else { throw new Exception("Scorer not valid: player does not play for either team."); }
            }
        }

        /// <summary>
        /// Assisting a goal for a player.
        /// </summary>
        /// <param name="assists">Observable Collection of players who assist a goal.</param>
        /// <exception cref="Exception">Player doesn't play for either team.</exception>
        private void AddAssist(ObservableCollection<Player> assists)
        {
            foreach (Player player in assists)
            {
                if (player.Team == HomeTeam)
                {
                    HomeAssists.Add(player);
                    player.AssistGoal(1);
                }
                else if (player.Team == AwayTeam)
                {
                    AwayAssists.Add(player);
                    player.AssistGoal(1);
                }
                else { throw new Exception("Assist not valid: player does not play for either team."); }
            }
        }

        /// <summary>
        /// Adds a player to the Observable Collection of yellow cards.
        /// </summary>
        /// <param name="yellowCards">Observable Collection of players who recieved a yellow card.</param>
        /// <exception cref="Exception">Player doesn't play for either team.</exception>
        private void AddYellowCards(ObservableCollection<Player> yellowCards)
        {
            foreach (Player player in yellowCards)
            {
                if (player.Team == HomeTeam)
                {
                    HomeYellowCards.Add(player);
                    player.YellowCard(1);
                }
                else if (player.Team == AwayTeam)
                {
                    AwayYellowCards.Add(player);
                    player.YellowCard(1);
                }
                else { throw new Exception("Yellow card not valid: player does not play for either team."); }
            }
        }

        /// <summary>
        /// Adds a player to the Observable Collection of red cards.
        /// </summary>
        /// <param name="redCards">Observable Collection of players who recieved a red card.</param>
        /// <exception cref="Exception">Player doesn't play for either team.</exception>
        private void AddRedCards(ObservableCollection<Player> redCards)
        {
            foreach (Player player in redCards)
            {
                if (player.Team == HomeTeam)
                {
                    HomeRedCards.Add(player);
                    player.RedCard();
                }
                else if (player.Team == AwayTeam)
                {
                    AwayRedCards.Add(player);
                    player.RedCard();
                }
                else { throw new Exception("Red card not valid: player does not play for either team."); }
            }
        }
    }
}
