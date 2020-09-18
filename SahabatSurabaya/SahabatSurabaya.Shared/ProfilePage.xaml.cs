using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace SahabatSurabaya
{ 
    public sealed partial class ProfilePage : Page
    {
        Session session;
        public ProfilePage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {

        }

        public void goToSubscriptionPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SubscriptionPage));
        }
    }
}
