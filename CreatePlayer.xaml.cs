using FootballScoresUI.models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePlayer : Page
    {
        private PlayerService _playerService = new PlayerService();

        private Team _selectedTeam;

        public CreatePlayer()
        {
            this.InitializeComponent();
            CreatePlayerLeagueDropdown.ItemsSource = DataStorage.Leagues;
            CreatePlayerPositionDropdown.ItemsSource = new string[] { "Goalkeeper", "Defender", "Midfielder", "Attacker" };
        }

        private void CreatePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            Player player = _playerService.CreatePlayer(CreatePlayerFirstNameInput.Text, CreatePlayerLastNameInput.Text, (Int32)CreatePlayerAgeInput.Value, (Int32)CreatePlayerKitNumberInput.Value, (string)CreatePlayerPositionDropdown.SelectedValue, _selectedTeam);
            CreatePlayerSubmitMessage.Text = $"{player.Name} added to {_selectedTeam.Name} successfully";
        }

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

        private void CreatePlayerTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedTeam = comboBox.SelectedItem as Team;
                if (selectedTeam != null)
                {
                    _selectedTeam = selectedTeam;
                }
            }
        }

        private void CreatePlayerNumberBoxInput_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            sender.Value = Math.Floor(args.NewValue);
        }
    }
}
