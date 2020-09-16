using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
                var oauthToken = await SecureStorage.GetAsync("oauth_token");
                if (oauthToken == null)
                {
                    var message = new MessageDialog("Null");
                    await message.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
            }
        }
    }
}
