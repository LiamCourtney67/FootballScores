using FootballScoresUI.models;
using System;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the ViewPlayer page.
    /// </summary>
    public sealed partial class ViewPlayer : Page
    {
        private Team _selectedTeam;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPlayer"/> class.
        /// </summary>
        public ViewPlayer()
        {
            this.InitializeComponent();
            ViewPlayerData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ViewPlayerErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            try { ViewPlayerLeagueDropdown.ItemsSource = DataStorage.Leagues; }
            catch (Exception)
            {
                ViewPlayerErrorMessage.Text = "Failed to get the data from the database.";
                ViewPlayerErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// Event handler for the ViewPlayerLeagueDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewPlayerLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null)
                {
                    ViewPlayerTeamDropdown.ItemsSource = selectedLeague.Teams;
                    ViewPlayerTeamDropdown.DisplayMemberPath = "Name";
                }
            }
        }

        /// <summary>
        /// Event handler for the ViewPlayerTeamDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewPlayerTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedTeam = comboBox.SelectedItem as Team;
                if (selectedTeam != null)
                {
                    _selectedTeam = selectedTeam;
                    ViewPlayerDropdown.ItemsSource = selectedTeam.Players;
                    ViewPlayerDropdown.DisplayMemberPath = "Name";
                }
            }
        }

        /// <summary>
        /// Event handler for the ViewPlayerDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewPlayerDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    ViewPlayerData.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ViewPlayerName.Text = selectedPlayer.Name;
                    ViewPlayerAge.Text = selectedPlayer.Age.ToString();
                    ViewPlayerKitNumber.Text = selectedPlayer.KitNumber.ToString();
                    ViewPlayerPosition.Text = selectedPlayer.Position;
                    ViewPlayerGoals.Text = selectedPlayer.GoalsScored.ToString();
                    ViewPlayerAssists.Text = selectedPlayer.Assists.ToString();
                    ViewPlayerGamesPlayed.Text = selectedPlayer.Team.GamesPlayed.ToString();
                    ViewPlayerYellowCards.Text = selectedPlayer.YellowCards.ToString();
                    ViewPlayerRedCards.Text = selectedPlayer.RedCards.ToString();

                    if (selectedPlayer.Position == "Goalkeeper" || selectedPlayer.Position == "Defender") { ViewPlayerCleanSheets.Text = selectedPlayer.CleanSheets.ToString(); }
                    else { ViewPlayerCleanSheets.Text = "N/A"; }
                }
            }
        }
    }
}
