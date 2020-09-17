using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;


namespace SahabatSurabaya.Shared
{
    public sealed partial class Splashscreen : Page
    {
        public Splashscreen()
        {
            this.InitializeComponent();
        }

        private async void pageLoaded(object Sender,RoutedEventArgs e)
        {
            try
            {
                var session = await SecureStorage.GetAsync("session");
                if (session == null)
                {
                    this.Frame.Navigate(typeof(LoginPage));
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
