using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI.models
{
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
        private string _matchData;

        // Player statistics.
        private ObservableCollection<Player> _homeScorers = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayScorers = new ObservableCollection<Player>();
        private ObservableCollection<Player> _homeYellowCards = new ObservableCollection<Player>();
        private ObservableCollection<Player> _homeRedCards = new ObservableCollection<Player>();

        private ObservableCollection<Player> _homeAssists = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayAssists = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayYellowCards = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayRedCards = new ObservableCollection<Player>();

        private ObservableCollection<Player> _cleanSheets = new ObservableCollection<Player>();

        private PlayerService _playerService = new PlayerService();
        private TeamService _teamService = new TeamService();

        public int MatchID { get => _matchID; set => _matchID = value; }
        public Team HomeTeam { get => _homeTeam; private set => _homeTeam = value; }
        public Team AwayTeam { get => _awayTeam; private set => _awayTeam = value; }
        public League League { get => _league; private set => _league = value; }
        public DateTime DatePlayed
        {
            get => _datePlayed;
            private set
            {
                if (value <= DateTime.Now)
                {
                    _datePlayed = value;
                }
                else
                {
                    throw new Exception("Date Played not valid: date cannot be in the future.");
                }
            }
        }
        public int HomeGoals { get => _homeGoals; private set => _homeGoals = value; }
        public int AwayGoals { get => _awayGoals; private set => _awayGoals = value; }
        public string Result { get => _result; private set => _result = value; }
        public string MatchData { get => _matchData; private set => _matchData = value; }

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

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.League = homeTeam.League;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = CalculateResult();
            this.MatchData = $"{DatePlayed.ToString("dd-MM-yyyy")}: {HomeTeam.Name} {HomeGoals} - {AwayGoals} {AwayTeam.Name}";

            // Check if teams are in the same league, if true add the match to the league and teams.
            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                this.HomeTeam.League.AddMatch(this);
                this.HomeTeam.AddMatch(this);
                this.AwayTeam.AddMatch(this);
                HomeTeam.AssignTeamStats(homeGoals, awayGoals);
                AwayTeam.AssignTeamStats(awayGoals, homeGoals);
                CheckCleenSheets();
            }
            else { throw new Exception("Teams not valid: teams are not in the same league."); }

        }

        public Match(Team homeTeam, Team awayTeam, DateTime datePlayed, int homeGoals, int awayGoals,
            ObservableCollection<Player> homeScorers, ObservableCollection<Player> homeAssists, ObservableCollection<Player> awayScorers,
            ObservableCollection<Player> awayAssists, ObservableCollection<Player> yellowCards, ObservableCollection<Player> redCards)
        {
            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
            this.League = homeTeam.League;
            this.DatePlayed = datePlayed;
            this.HomeGoals = homeGoals;
            this.AwayGoals = awayGoals;
            this.Result = CalculateResult();
            this.MatchData = $"{DatePlayed.ToString("dd-MM-yyyy")}: {HomeTeam.Name} {HomeGoals} - {AwayGoals} {AwayTeam.Name}";

            // Check if teams are in the same league, if true add the match to the league and teams.
            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                _homeScorers = homeScorers;
                _homeAssists = homeAssists;
                _awayScorers = awayScorers;
                _awayAssists = awayAssists;
                AddYellowCards(yellowCards);
                AddRedCards(redCards);
                this.HomeTeam.League.AddMatch(this);
                this.HomeTeam.AddMatch(this);
                this.AwayTeam.AddMatch(this);
                HomeTeam.AssignTeamStats(homeGoals, awayGoals);
                AwayTeam.AssignTeamStats(awayGoals, homeGoals);
                CheckCleenSheets();
            }
            else { throw new Exception("Teams not valid: teams are not in the same league."); }
        }

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
            this.MatchData = $"{DatePlayed.ToString("dd-MM-yyyy")}: {HomeTeam.Name} {HomeGoals} - {AwayGoals} {AwayTeam.Name}";

            if (HomeTeam.League.LeagueID == AwayTeam.League.LeagueID)
            {
                _homeScorers = homeScorers;
                _homeAssists = homeAssists;
                _awayScorers = awayScorers;
                _awayAssists = awayAssists;
                AddYellowCards(yellowCards);
                AddRedCards(redCards);
                this.HomeTeam.League.AddMatch(this);
                this.HomeTeam.AddMatch(this);
                this.AwayTeam.AddMatch(this);
            }
            else { throw new Exception("Teams not valid: teams are not in the same league."); }
        }

        private string CalculateResult()
        {
            if (HomeGoals > AwayGoals)
            {
                return "Home";
            }
            else if (AwayGoals > HomeGoals)
            {
                return "Away";
            }
            else
            {
                return "Draw";
            }
        }

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
                else
                {
                    throw new Exception("Yellow card not valid: player does not play for either team.");
                }
            }
        }

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
                else
                {
                    throw new Exception("Red card not valid: player does not play for either team.");
                }
            }
        }
    }
}
