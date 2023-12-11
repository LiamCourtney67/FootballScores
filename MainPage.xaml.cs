using FootballScoresUI.models;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace FootballScoresUI
{
    /// <summary>
    /// Main page of the application used as the navigation.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationViewItem _lastItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when an item within the navigation view is selected.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the invoke event.</param>
        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItemContainer as NavigationViewItem;
            if (item == null || item == _lastItem) return;

            var clickedView = item.Tag?.ToString() ?? "SettingsView";
            if (!NavigateToView(clickedView)) return;
            _lastItem = item;
        }

        /// <summary>
        /// Navigates to the view that was clicked.
        /// </summary>
        /// <param name="clickedView">Page to navigate to.</param>
        /// <returns></returns>
        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly().GetType($"FootballScoresUI.{clickedView}");

            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
                return false;

            ContentFrame.Navigate(view, new EntranceNavigationTransitionInfo());
            return true;
        }

        /// <summary>
        /// Navigates to the home page when the navigation view is loaded.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the load event.</param>
        private void NavigationView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (NavigationViewItemBase item in NavigationView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Home")
                {
                    NavigationView.SelectedItem = item;
                    break;
                }
            }

            ContentFrame.Navigate(typeof(Home));
        }

        /// <summary>
        /// Navigates back when the back button is clicked.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the back request event.</param>
        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) { if (ContentFrame.CanGoBack) { ContentFrame.GoBack(); } }
    }
}
