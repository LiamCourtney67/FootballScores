using FootballScoresUI.models;
using System;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the ViewLeague page.
    /// </summary>
    public sealed partial class ViewLeague : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLeague"/> class.
        /// </summary>
        public ViewLeague()
        {
            this.InitializeComponent();
            ViewLeagueData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ViewLeagueErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            try { ViewLeagueDropdown.ItemsSource = DataStorage.Leagues; }
            catch (Exception)
            {
                ViewLeagueErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
                ViewLeagueErrorMessage.Text = "Failed to get the data from the database.";
            }
        }

        /// <summary>
        /// Event handler for the ViewLeagueDropdown_SelectionChanged event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null)
                {
                    ViewLeagueData.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ViewLeagueTeamItemsControl.ItemsSource = selectedLeague.Teams;
                }
            }
        }
    }
}
