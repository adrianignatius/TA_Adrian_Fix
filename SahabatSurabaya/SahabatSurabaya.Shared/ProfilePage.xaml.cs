using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

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
        public async void goToSubscriptionPage(object sender, RoutedEventArgs e)
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
