using FootballScoresUI.models;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTeam : Page
    {
        private TeamService _teamService = new TeamService();

        private League _selectedLeague;

        public CreateTeam()
        {
            this.InitializeComponent();
            CreateTeamLeagueDropdown.ItemsSource = DataStorage.Leagues;
        }

        private void CreateTeamButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Team team = _teamService.CreateTeam(CreateTeamInput.Text, _selectedLeague);
            CreateTeamSubmitMessage.Text = $"{team.Name} added to {_selectedLeague.Name} successfully";
        }

        private void CreateTeamLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null)
                {
                    _selectedLeague = selectedLeague;
                }
            }
        }
    }
}
