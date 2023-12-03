using FootballScoresUI.models;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewPlayer : Page
    {
        private Team _selectedTeam;

        public ViewPlayer()
        {
            this.InitializeComponent();
            ViewPlayerData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ViewPlayerLeagueDropdown.ItemsSource = DataStorage.Leagues;
        }

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

                    if (selectedPlayer.Position == "Goalkeeper" || selectedPlayer.Position == "Defender")
                    {
                        ViewPlayerCleanSheets.Text = selectedPlayer.CleanSheets.ToString();
                    }
                    else
                    {
                        ViewPlayerCleanSheets.Text = "N/A";
                    }
                }
            }
        }
    }
}
