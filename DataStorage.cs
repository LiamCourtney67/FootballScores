using FootballScoresUI.models;
using System.Collections.ObjectModel;

namespace FootballScoresUI
{
    public static class DataStorage
    {
        public static LeagueService leagueService = new LeagueService();
        public static ObservableCollection<League> Leagues { get; set; } = leagueService.GetAllLeagues();
    }
}