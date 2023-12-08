using FootballScoresUI.models;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the CreatePlayer page.
    /// </summary>
    public sealed partial class CreatePlayer : Page
    {
        private PlayerService _playerService = new PlayerService();

        private Team _selectedTeam;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlayer"/> class.
        /// </summary>
        public CreatePlayer()
        {
            this.InitializeComponent();
            CreatePlayerPositionDropdown.ItemsSource = new string[] { "Goalkeeper", "Defender", "Midfielder", "Attacker" };

            try { CreatePlayerLeagueDropdown.ItemsSource = DataStorage.Leagues; }
            catch (Exception) { CreatePlayerSubmitMessage.Text = "Failed to get the data from the database."; }
        }

        /// <summary>
        /// Event handler for the CreatePlayerButton click event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CreatePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTeam != null)
                {
                    Player player = _playerService.CreatePlayer(CreatePlayerFirstNameInput.Text, CreatePlayerLastNameInput.Text, (Int32)CreatePlayerAgeInput.Value, (Int32)CreatePlayerKitNumberInput.Value, (string)CreatePlayerPositionDropdown.SelectedValue, _selectedTeam);
                    CreatePlayerSubmitMessage.Text = $"{player.Name} added to {_selectedTeam.Name} successfully";
                    CreatePlayerFirstNameInput.Text = "";
                    CreatePlayerLastNameInput.Text = "";
                    CreatePlayerAgeInput.Text = "";
                    CreatePlayerPositionDropdown.SelectedItem = null;
                    CreatePlayerKitNumberInput.Text = "";
                }
                else { throw new Exception("Team not valid: select a team first."); }
            }
            catch (Exception ex) { CreatePlayerSubmitMessage.Text = $"{ex.Message}"; }
        }

        /// <summary>
        /// Event handler for the CreatePlayerLeagueDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreatePlayerLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null)
                {
                    CreatePlayerTeamDropdown.ItemsSource = selectedLeague.Teams;
                    CreatePlayerTeamDropdown.DisplayMemberPath = "Name";
                }
            }
        }

        /// <summary>
        /// Event handler for the CreatePlayerTeamDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreatePlayerTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedTeam = comboBox.SelectedItem as Team;
                if (selectedTeam != null) { _selectedTeam = selectedTeam; }
            }
        }

        /// <summary>
        /// Event handler for the input of any NumberBoxes values changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the change event.</param>
        private void CreatePlayerNumberBoxInput_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args) { sender.Value = Math.Floor(args.NewValue); }
    }
}
