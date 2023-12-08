using FootballScoresUI.models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the CreateMatch page.
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateMatch"/> class.
        /// </summary>
        public CreateMatch()
        {
            this.InitializeComponent();
            try { CreateMatchLeagueDropdown.ItemsSource = DataStorage.Leagues; }
            catch (Exception) { CreateMatchSubmitMessage.Text = "Failed to get the data from the database."; }
        }

        /// <summary>
        /// Event handler for the CreateMatchButton click event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the click event.</param>
        private void CreateMatchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedHomeTeam != null && _selectedAwayTeam != null)
                {
                    if (CreateMatchDateInput.SelectedDate != null)
                    {
                        Match match = _matchService.CreateMatch(_selectedHomeTeam, _selectedAwayTeam, CreateMatchDateInput.Date.Date, (int)(CreateMatchHomeGoalsInput.Value),
                            (int)(CreateMatchAwayGoalsInput.Value), _homeScorers, _homeAssists, _awayScorers, _awayAssists, _yellowCards, _redCards);
                        CreateMatchSubmitMessage.Text = $"{match.HomeTeam.Name} {match.HomeGoals} - {match.AwayGoals} {match.AwayTeam.Name} added to {match.League.Name} successfully";

                        CreateMatchHomeTeamDropdown.SelectedItem = null;
                        CreateMatchHomeGoalsInput.Value = 0;

                        CreateMatchAwayTeamDropdown.SelectedItem = null;
                        CreateMatchAwayGoalsInput.Value = 0;

                        CreateMatchDateInput.SelectedDate = null;

                        _homeScorers.Clear();
                        _homeAssists.Clear();
                        _awayScorers.Clear();
                        _awayAssists.Clear();
                        _yellowCards.Clear();
                        _redCards.Clear();
                    }
                    else { throw new Exception("Date Played not valid: select a date."); }
                }
                else { throw new Exception("Teams not valid: select a home and away team."); }
            }
            catch (Exception ex) { CreateMatchSubmitMessage.Text = $"{ex.Message}"; }
        }

        /// <summary>
        /// Event handler for the CreateMatchLeagueDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
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

        /// <summary>
        /// Event handler for the CreateMatchHomeTeamDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreateMatchHomeTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedHomeTeam = CreateMatchHomeTeamDropdown.SelectedItem as Team;
            if (_selectedHomeTeam != null)
            {
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

        }

        /// <summary>
        /// Event handler for the CreateMatchAwayTeamDropdown selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreateMatchAwayTeamDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedAwayTeam = CreateMatchAwayTeamDropdown.SelectedItem as Team;
            if (_selectedAwayTeam != null)
            {
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
        }

        /// <summary>
        /// Event handler for the CreateMatchHomeScorerInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreateMatchHomeScorerInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_homeScorers.Count() < CreateMatchHomeGoalsInput.Value)
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

        /// <summary>
        /// Event handler for the CreateMatchHomeAssistInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreateMatchHomeAssistInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_homeAssists.Count() < CreateMatchHomeGoalsInput.Value)
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

        /// <summary>
        /// Event handler for the CreateMatchHomeYellowCardInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
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

        /// <summary>
        /// Event handler for the CreateMatchHomeRedCardInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
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

        /// <summary>
        /// Event handler for the CreateMatchAwayScorerInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreateMatchAwayScorerInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_awayScorers.Count() < CreateMatchAwayGoalsInput.Value)
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

        /// <summary>
        /// Event handler for the CreateMatchAwayAssistInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
        private void CreateMatchAwayAssistInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedPlayer = comboBox.SelectedItem as Player;
                if (selectedPlayer != null)
                {
                    if (_awayAssists.Count() < CreateMatchAwayGoalsInput.Value)
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

        /// <summary>
        /// Event handler for the CreateMatchAwayYellowCardInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
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

        /// <summary>
        /// Event handler for the CreateMatchAwayRedCardInput selection changed event.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection event.</param>
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
