using FootballScoresUI.models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the ViewMatch page.
    /// </summary>
    public sealed partial class ViewMatch : Page
    {
        private Team _selectedFirstTeam;
        private Team _selectedSecondTeam;
        private Match _selectedMatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMatch"/> class.
        /// </summary>
        public ViewMatch()
        {
            this.InitializeComponent();
            ViewMatchData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ViewMatchErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            try { ViewMatchLeagueDropdown.ItemsSource = DataStorage.Leagues; }
            catch (Exception)
            {
                ViewMatchErrorMessage.Text = "Failed to get the data from the database.";
                ViewMatchErrorMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// Event handler for the ViewMatchLeagueDropdown selection changed event.
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
                    ViewMatchFirstTeamDropdown.ItemsSource = selectedLeague.Teams;
                    ViewMatchFirstTeamDropdown.DisplayMemberPath = "Name";
                    ViewMatchSecondTeamDropdown.ItemsSource = selectedLeague.Teams;
                    ViewMatchSecondTeamDropdown.DisplayMemberPath = "Name";
                }
            }
        }

        /// <summary>
        /// Event handler for the ViewMatchFirstTeamDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewMatchFirstTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedFirstTeam = comboBox.SelectedItem as Team;
                if (selectedFirstTeam != null)
                {
                    _selectedFirstTeam = selectedFirstTeam;
                    if (_selectedFirstTeam != null)
                    {
                        if (_selectedFirstTeam != _selectedSecondTeam)
                        {
                            ViewMatchDateAndScoreDropdown.ItemsSource = _selectedFirstTeam.Matches;
                            ViewMatchDateAndScoreDropdown.DisplayMemberPath = "MatchData";
                        }
                        else { comboBox.SelectedItem = null; }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for the ViewMatchSecondTeamDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewMatchSecondDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                if (_selectedFirstTeam != null)
                {
                    var selectedSecondTeam = comboBox.SelectedItem as Team;
                    if (selectedSecondTeam != null)
                    {
                        _selectedSecondTeam = selectedSecondTeam;
                        if (_selectedSecondTeam != null && _selectedFirstTeam != null)
                        {
                            if (_selectedFirstTeam != _selectedSecondTeam)
                            {
                                ViewMatchDateAndScoreDropdown.ItemsSource = FilterMatches(_selectedFirstTeam, _selectedSecondTeam);
                                ViewMatchDateAndScoreDropdown.DisplayMemberPath = "MatchData";
                            }
                            else { comboBox.SelectedItem = null; }
                        }
                    }
                }
                else { comboBox.SelectedItem = null; }
            }
        }

        /// <summary>
        /// Filters the matches for only the two teams.
        /// </summary>
        /// <param name="firstTeam">Team object of the first team.</param>
        /// <param name="secondTeam">Team object of the second team.</param>
        /// <returns>An Observable Collection of filtered matches.</returns>
        private ObservableCollection<Match> FilterMatches(Team firstTeam, Team secondTeam)
        {
            if (firstTeam == null || secondTeam == null) { return null; }

            var filteredMatches = firstTeam.Matches.Where(m => m.HomeTeam == secondTeam || m.AwayTeam == secondTeam).ToList();
            return new ObservableCollection<Match>(filteredMatches);
        }

        /// <summary>
        /// Event handler for the ViewMatchDateDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void ViewMatchDateDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                _selectedMatch = comboBox.SelectedItem as Match;
                if (_selectedMatch != null)
                {
                    ViewMatchData.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ViewMatchHomeTeamName.Text = _selectedMatch.HomeTeam.Name;
                    ViewMatchHomeTeamGoals.Text = _selectedMatch.HomeGoals.ToString();
                    ViewMatchHomeTeamScorersItemsControl.ItemsSource = _selectedMatch.HomeScorers;
                    ViewMatchHomeTeamAssistsItemsControl.ItemsSource = _selectedMatch.HomeAssists;
                    ViewMatchHomeTeamYellowCardsItemsControl.ItemsSource = _selectedMatch.HomeYellowCards;
                    ViewMatchHomeTeamRedCardsItemsControl.ItemsSource = _selectedMatch.HomeRedCards;

                    ViewMatchAwayTeamName.Text = _selectedMatch.AwayTeam.Name;
                    ViewMatchAwayTeamGoals.Text = _selectedMatch.AwayGoals.ToString();
                    ViewMatchAwayTeamScorersItemsControl.ItemsSource = _selectedMatch.AwayScorers;
                    ViewMatchAwayTeamAssistsItemsControl.ItemsSource = _selectedMatch.AwayAssists;
                    ViewMatchAwayTeamYellowCardsItemsControl.ItemsSource = _selectedMatch.AwayYellowCards;
                    ViewMatchAwayTeamRedCardsItemsControl.ItemsSource = _selectedMatch.AwayRedCards;

                }
            }
        }
    }
}
