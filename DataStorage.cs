using FootballScoresUI.models;
using System;
using System.Collections.ObjectModel;

namespace FootballScoresUI
{
    /// <summary>
    /// Static class to store data for the application.
    /// </summary>
    public static class DataStorage
    {
        public static LeagueService LeagueService;
        public static ObservableCollection<League> Leagues;

        /// <summary>
        /// Static constructor to initialise the LeagueService and Leagues.
        /// </summary>
        static DataStorage()
        {
            try
            {
                LeagueService = new LeagueService();
                Leagues = LeagueService.GetAllLeagues();
            }
            catch (Exception) { throw; }
        }
    }
}