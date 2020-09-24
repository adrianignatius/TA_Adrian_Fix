using SahabatSurabaya.Shared;
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
        User userLogin;
        public ProfilePage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            txtNotelpUser.Text = userLogin.telpon_user;
        }

        private void goToSubscriptionPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SubscriptionPage));
        }
        
        private void changePassword(object sender,RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChangePasswordPage));
        }
    }
}
