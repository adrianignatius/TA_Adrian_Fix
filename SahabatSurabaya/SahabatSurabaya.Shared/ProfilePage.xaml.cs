using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace SahabatSurabaya
{ 
    public sealed partial class ProfilePage : Page
    {
        User userLogin;
        Session session;
        public ProfilePage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        private async void pageLoaded(object sender, RoutedEventArgs e)
        {
        }
        public void goToSubscriptionPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SubscriptionPage));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            userLogin = e.Parameter as User;
            var m = new MessageDialog(userLogin.nama_user);
            await m.ShowAsync();

        }
    }
}
