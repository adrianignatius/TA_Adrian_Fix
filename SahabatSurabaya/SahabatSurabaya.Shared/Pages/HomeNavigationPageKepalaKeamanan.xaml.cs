using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

namespace SahabatSurabaya.Shared.Pages
{
    public sealed partial class HomeNavigationPageKepalaKeamanan : Page
    {
        public HomeNavigationPageKepalaKeamanan()
        {
            this.InitializeComponent();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "home")
                {
                    NavView.SelectedItem = item;
                    NavView_Navigate(item as NavigationViewItem);
                    break;
                }
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(LoginPage));
            }
            else
            {
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "AllLostFoundReportPage":
                    this.Frame.Navigate(typeof(AllLostFoundReportPage));
                    break;
                case "AllCrimeReportPage":
                    this.Frame.Navigate(typeof(AllCrimeReportPage));
                    break;
                case "AreaReportPage":
                    this.Frame.Navigate(typeof(LaporanKepalaKeamananPage));
                    break;
                case "SignOut":
                    this.Frame.Navigate(typeof(LoginPage));
                    SecureStorage.Remove("jwt_token");
                    break;
            }
        }
    }
}
