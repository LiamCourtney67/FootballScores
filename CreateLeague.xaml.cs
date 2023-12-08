using FootballScoresUI.models;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the CreateLeague page.
    /// </summary>
    public sealed partial class CreateLeague : Page
    {
        private LeagueService _leagueService = new LeagueService();
        private TeamService _teamService = new TeamService();

        private League _createdLeague = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateLeague"/> class.
        /// </summary>
        public CreateLeague() { this.InitializeComponent(); }

        /// <summary>
        /// Event handler for the CreateLeagueButton click event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CreateLeagueButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (!DataStorage.Leagues.Any(league => league.Name == CreateLeagueInput.Text))      // Checks if the league name already exists.
                {
                    _createdLeague = _leagueService.CreateLeague(CreateLeagueInput.Text);
                    DataStorage.Leagues.Add(_createdLeague);
                    CreateLeagueSubmitMessage.Text = $"{_createdLeague.Name} created successfully";
                    CreateLeagueAddTeamInput.PlaceholderText = $"Enter a team name to add to {_createdLeague.Name}...";
                    CreateLeagueInput.Text = "";
                }
            }
            catch (Exception ex) { CreateLeagueSubmitMessage.Text = $"{ex.Message}"; }
        }

        /// <summary>
        /// Event handler for the CreateLeagueAddTeamButton click event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CreateLeagueAddTeamButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (_createdLeague != null)
                {
                    Team team = _teamService.CreateTeam(CreateLeagueAddTeamInput.Text, _createdLeague);
                    CreateLeagueSubmitMessage.Text = $"{team.Name} added to {_createdLeague.Name} successfully";
                    CreateLeagueAddTeamInput.Text = "";
                }
                else { throw new Exception("League not valid: create a league first."); }
            }
            catch (Exception ex) { CreateLeagueSubmitMessage.Text = $"{ex.Message}"; }
        }
    }
}
