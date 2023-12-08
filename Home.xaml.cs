using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FootballScoresUI
{
    /// <summary>
    /// View model for the Home page.
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            HomeSubmitMessage.Text = "This will either be a succes or error message and the options will be cleared...";
            HomeDropdown.PlaceholderText = "Dropdowns are used to select options...";
            HomeInput.Text = "";
        }

        private void HomeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HomeDropdown.SelectedItem = null;
            HomeDropdown.PlaceholderText = "The selected item will be displayed here...";
        }
    }
}
