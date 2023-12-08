using FootballScoresUI.models;
using System;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the ViewTeam page.
    /// </summary>
    public sealed partial class ViewTeam : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewTeam"/> class.
        /// </summary>
        /// <exception cref="Exception">Failed to get the data from the database.</exception>"
        public ViewTeam()
        {
            this.InitializeComponent();
            ViewTeamData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ViewTeamErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            try { ViewTeamLeagueDropdown.ItemsSource = DataStorage.Leagues; }
            catch (Exception)
            {
                ViewTeamErrorMessage.Text = "Failed to get the data from the database.";
                ViewTeamErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// Event handler for the ViewTeamLeagueDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewTeamLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null)
                {
                    ViewTeamDropdown.ItemsSource = selectedLeague.Teams;
                    ViewTeamDropdown.DisplayMemberPath = "Name";
                }
            }
        }

        /// <summary>
        /// Event handler for the ViewTeamDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedTeam = comboBox.SelectedItem as Team;
                if (selectedTeam != null)
                {
                    ViewTeamData.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ViewTeamName.Text = selectedTeam.Name;
                    ViewTeamGamesPlayed.Text = selectedTeam.GamesPlayed.ToString();
                    ViewTeamGamesWon.Text = selectedTeam.GamesWon.ToString();
                    ViewTeamGamesDrawn.Text = selectedTeam.GamesDrawn.ToString();
                    ViewTeamGamesLost.Text = selectedTeam.GamesLost.ToString();
                    ViewTeamGoalsFor.Text = selectedTeam.GoalsFor.ToString();
                    ViewTeamGoalsAgainst.Text = selectedTeam.GoalsAgainst.ToString();
                    ViewTeamGoalDifference.Text = selectedTeam.GoalDifference.ToString();
                    ViewTeamPoints.Text = selectedTeam.Points.ToString();

                    if (selectedTeam.Players.Count > 0)
                    {
                        ViewTeamPlayerItemsControl.ItemsSource = selectedTeam.Players;
                        ViewTeamPlayerData.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else { ViewTeamPlayerData.Visibility = Windows.UI.Xaml.Visibility.Collapsed; }
                }
            }
        }
    }
}
