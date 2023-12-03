using FootballScoresUI.models;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballScoresUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewLeague : Page
    {

        public ViewLeague()
        {
            this.InitializeComponent();
            ViewLeagueDropdown.ItemsSource = DataStorage.Leagues;
            ViewLeagueData.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ViewLeagueDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                var selectedLeague = comboBox.SelectedItem as League;
                if (selectedLeague != null)
                {
                    ViewLeagueData.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ViewLeagueTeamItemsControl.ItemsSource = selectedLeague.Teams;
                }
            }
        }
    }
}
