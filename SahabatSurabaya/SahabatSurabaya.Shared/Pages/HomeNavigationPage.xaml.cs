using SahabatSurabaya.Shared;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class HomeNavigationPage : Page
    {
        Session session;
        public HomeNavigationPage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // you can also add items in code behind
            //NavView.MenuItems.Add(new NavigationViewItemSeparator());
            
            //NavView.MenuItems.Add(new NavigationViewItem()
            //{ Content = "My content", Icon = new SymbolIcon(Symbol.Folder), Tag = "content" });

            // set the initial SelectedItem 
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

                case "MakeLostFoundReportPage":
                    this.Frame.Navigate(typeof(MakeLostFoundReportPage));
                    break;

                case "MakeCrimeReportPage":
                    this.Frame.Navigate(typeof(MakeCrimeReportPage));
                    break;

                case "SignOut":
                    SecureStorage.Remove("jwt_token");
                    this.Frame.BackStack.Clear();
                    this.Frame.Navigate(typeof(LoginPage));
                    break;

                case "PoliceStationPage":
                    ContentFrame.Navigate(typeof(PoliceStationPage));
                    break;

                case "ProfilePage":
                    ContentFrame.Navigate(typeof(ProfilePage));
                    break;

                case "chatPage":
                    ContentFrame.Navigate(typeof(ChatPage));
                    break;

                case "contactPage":
                    ContentFrame.Navigate(typeof(ContactPage));
                    break;

                case "historyPage":
                    this.Frame.Navigate(typeof(HistoryPage));
                    break;
            }
        }
    }
}
