using FootballScoresUI.models;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewTeam : Page
    {
        public ViewTeam()
        {
            this.InitializeComponent();
            ViewTeamLeagueDropdown.ItemsSource = DataStorage.Leagues;
            ViewTeamData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

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

                    ViewTeamPlayerItemsControl.ItemsSource = selectedTeam.Players;
                }
            }
        }
    }
}
