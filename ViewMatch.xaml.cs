using FootballScoresUI.models;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewMatch : Page
    {
        private Team _selectedFirstTeam;
        private Team _selectedSecondTeam;
        private Match _selectedMatch;

        public ViewMatch()
        {
            this.InitializeComponent();
            ViewMatchData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ViewMatchLeagueDropdown.ItemsSource = DataStorage.Leagues;
        }

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
                        else
                        {
                            comboBox.SelectedItem = null;
                        }
                    }
                }
            }
        }

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
                            else
                            {
                                comboBox.SelectedItem = null;
                            }
                        }
                    }
                }
                else
                {
                    comboBox.SelectedItem = null;
                }
            }
        }

        private ObservableCollection<Match> FilterMatches(Team firstTeam, Team secondTeam)
        {
            if (firstTeam == null || secondTeam == null)
            {
                return null;
            }

            var filteredMatches = firstTeam.Matches.Where(m => m.HomeTeam == secondTeam || m.AwayTeam == secondTeam).ToList();
            return new ObservableCollection<Match>(filteredMatches);
        }

        private void ViewMatchDateDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedMatch = comboBox.SelectedItem as Match;
                if (selectedMatch != null)
                {
                    _selectedMatch = selectedMatch;
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
}
