using FootballScoresUI.models;
using System;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    public sealed partial class CreateLeague : Page
    {
        private LeagueService _leagueService = new LeagueService();
        private TeamService _teamService = new TeamService();

        private League _createdLeague;

        public CreateLeague()
        {
            this.InitializeComponent();
        }

        private void CreateLeagueButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                DataStorage.Leagues.Add(_createdLeague = _leagueService.CreateLeague(CreateLeagueInput.Text));
                CreateLeagueSubmitMessage.Text = $"{CreateLeagueInput.Text} created successfully";
            }
            catch (Exception ex)
            {
                CreateLeagueSubmitMessage.Text = $"{ex.Message}";
            }
        }

        private void AddTeamButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                Team team = _teamService.CreateTeam(AddTeamInput.Text, _createdLeague);
                CreateLeagueSubmitMessage.Text = $"{team.Name} added to {_createdLeague.Name} successfully";
            }
            catch (Exception ex)
            {
                CreateLeagueSubmitMessage.Text = $"{ex.Message}";
            }
        }
    }
}
