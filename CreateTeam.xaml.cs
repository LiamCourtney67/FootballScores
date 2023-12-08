using FootballScoresUI.models;
using System;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the CreateTeam page.
    /// </summary>
    public sealed partial class CreateTeam : Page
    {
        private TeamService _teamService = new TeamService();

        private League _selectedLeague;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTeam"/> class.
        /// </summary>
        public CreateTeam()
        {
            this.InitializeComponent();
            try { CreateTeamLeagueDropdown.ItemsSource = DataStorage.Leagues; }
            catch (Exception) { CreateTeamSubmitMessage.Text = "Failed to get the data from the database."; }
        }

        /// <summary>
        /// Event handler for the CreateTeamButton click event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CreateTeamButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                if (_selectedLeague != null)
                {
                    Team team = _teamService.CreateTeam(CreateTeamInput.Text, _selectedLeague);
                    CreateTeamSubmitMessage.Text = $"{team.Name} added to {_selectedLeague.Name} successfully";
                    CreateTeamInput.Text = "";
                }
                else { throw new Exception("League not valid: select a league first."); }
            }
            catch (Exception ex) { CreateTeamSubmitMessage.Text = $"{ex.Message}"; }
        }

        /// <summary>
        /// Event handler for the CreateTeamLeagueDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreateTeamLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null) { _selectedLeague = selectedLeague; }
            }
        }
    }
}
