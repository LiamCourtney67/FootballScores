using FootballScoresUI.models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateMatch : Page
    {
        private MatchService _matchService = new MatchService();

        private Team _selectedHomeTeam;
        private Team _selectedAwayTeam;

        private ObservableCollection<Player> _homeScorers = new ObservableCollection<Player>();
        private ObservableCollection<Player> _homeAssists = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayScorers = new ObservableCollection<Player>();
        private ObservableCollection<Player> _awayAssists = new ObservableCollection<Player>();
        private ObservableCollection<Player> _yellowCards = new ObservableCollection<Player>();
        private ObservableCollection<Player> _redCards = new ObservableCollection<Player>();


        public CreateMatch()
        {
            this.InitializeComponent();
            CreateMatchLeagueDropdown.ItemsSource = DataStorage.Leagues;
        }

        private void CreateMatchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Match match = _matchService.CreateMatch(_selectedHomeTeam, _selectedAwayTeam, CreateMatchDateInput.Date.Date, int.Parse(CreateMatchHomeGoalsInput.Text),
                    int.Parse(CreateMatchAwayGoalsInput.Text), _homeScorers, _homeAssists, _awayScorers, _awayAssists, _yellowCards, _redCards);
                CreateMatchSubmitMessage.Text = $"{match.HomeTeam.Name} {match.HomeGoals} - {match.AwayGoals} {match.AwayTeam.Name} added to {match.League.Name} successfully";
            }
            catch (Exception ex)
            {
                CreateMatchSubmitMessage.Text = $"{ex.Message}";
            }
        }

        private void CreateMatchLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null)
                {
                    CreateMatchHomeTeamDropdown.ItemsSource = selectedLeague.Teams;
                    CreateMatchHomeTeamDropdown.DisplayMemberPath = "Name";
                    CreateMatchAwayTeamDropdown.ItemsSource = selectedLeague.Teams;
                    CreateMatchAwayTeamDropdown.DisplayMemberPath = "Name";
                }
            }
        }

        private void CreateMatchHomeTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedHomeTeam = CreateMatchHomeTeamDropdown.SelectedItem as Team;
            if (_selectedHomeTeam.Players.Count != 0)
            {
                CreateMatchHomeScorerInput.ItemsSource = _selectedHomeTeam.Players;
                CreateMatchHomeScorerInput.DisplayMemberPath = "Name";
                CreateMatchHomeAssistInput.ItemsSource = _selectedHomeTeam.Players;
                CreateMatchHomeAssistInput.DisplayMemberPath = "Name";
                CreateMatchHomeYellowCardInput.ItemsSource = _selectedHomeTeam.Players;
                CreateMatchHomeYellowCardInput.DisplayMemberPath = "Name";
                CreateMatchHomeRedCardInput.ItemsSource = _selectedHomeTeam.Players;
                CreateMatchHomeRedCardInput.DisplayMemberPath = "Name";
            }

        }

        private void CreateMatchAwayTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedAwayTeam = CreateMatchAwayTeamDropdown.SelectedItem as Team;
            if (_selectedAwayTeam.Players.Count != 0)
            {
                CreateMatchAwayScorerInput.ItemsSource = _selectedAwayTeam.Players;
                CreateMatchAwayScorerInput.DisplayMemberPath = "Name";
                CreateMatchAwayAssistInput.ItemsSource = _selectedAwayTeam.Players;
                CreateMatchAwayAssistInput.DisplayMemberPath = "Name";
                CreateMatchAwayYellowCardInput.ItemsSource = _selectedAwayTeam.Players;
                CreateMatchAwayYellowCardInput.DisplayMemberPath = "Name";
                CreateMatchAwayRedCardInput.ItemsSource = _selectedAwayTeam.Players;
                CreateMatchAwayRedCardInput.DisplayMemberPath = "Name";
            }
        }

        private void CreateMatchHomeScorerInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_homeScorers.Count(player => player.PlayerID == selectedPlayer.PlayerID) < CreateMatchHomeGoalsInput.Value)
                    {
                        _homeScorers.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedHomeTeam.Name} scorers.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{_selectedHomeTeam.Name} has only scored {CreateMatchHomeGoalsInput.Value} goals.";
                        comboBox.SelectedItem = null;
                    }

                }
            }
        }

        private void CreateMatchHomeAssistInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_homeAssists.Count(player => player.PlayerID == selectedPlayer.PlayerID) < CreateMatchHomeGoalsInput.Value)
                    {
                        _homeAssists.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedHomeTeam.Name} assists.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{_selectedHomeTeam.Name} has only scored {CreateMatchHomeGoalsInput.Value} goals.";
                        comboBox.SelectedItem = null;
                    }
                }
            }
        }

        private void CreateMatchHomeYellowCardInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_yellowCards.Count(player => player.PlayerID == selectedPlayer.PlayerID) < 2)
                    {
                        _yellowCards.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedHomeTeam.Name} yellow cards.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} already has 2 yellow cards.";
                        comboBox.SelectedItem = null;
                    }
                }
            }
        }

        private void CreateMatchHomeRedCardInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (!_redCards.Contains(selectedPlayer))
                    {
                        _redCards.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedHomeTeam.Name} red cards.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} already has a red card.";
                        comboBox.SelectedItem = null;
                    }
                }
            }
        }

        private void CreateMatchAwayScorerInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_awayAssists.Count(player => player.PlayerID == selectedPlayer.PlayerID) < CreateMatchAwayGoalsInput.Value)
                    {
                        _awayScorers.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedAwayTeam.Name} scorers.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{_selectedAwayTeam.Name} has only scored {CreateMatchAwayGoalsInput.Value} goals.";
                        comboBox.SelectedItem = null;
                    }
                }
            }
        }

        private void CreateMatchAwayAssistInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_awayAssists.Count(player => player.PlayerID == selectedPlayer.PlayerID) < CreateMatchAwayGoalsInput.Value)
                    {
                        _awayAssists.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedAwayTeam.Name} assists.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{_selectedAwayTeam.Name} has only scored {CreateMatchAwayGoalsInput.Value} goals.";
                        comboBox.SelectedItem = null;
                    }
                }
            }
        }

        private void CreateMatchAwayYellowCardInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_yellowCards.Count(player => player.PlayerID == selectedPlayer.PlayerID) < 2)
                    {
                        _yellowCards.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedAwayTeam.Name} yellow cards.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} already has 2 yellow cards.";
                        comboBox.SelectedItem = null;
                    }
                }
            }
        }

        private void CreateMatchAwayRedCardInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (!_redCards.Contains(selectedPlayer))
                    {
                        _redCards.Add(selectedPlayer);
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} added to {_selectedAwayTeam.Name} red cards.";
                        comboBox.SelectedItem = null;
                    }
                    else
                    {
                        CreateMatchSubmitMessage.Text = $"{selectedPlayer.Name} already has a red card.";
                        comboBox.SelectedItem = null;
                    }
                }
            }
        }
    }
}
