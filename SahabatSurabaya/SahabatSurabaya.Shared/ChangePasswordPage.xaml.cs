using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel.Channels;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace SahabatSurabaya.Shared
{

    public sealed partial class ChangePasswordPage : Page
    {
        Session session;
        User userLogin;
        public ChangePasswordPage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        private void pageLoaded(object sender,RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
        }

        private async void changePassword (object sender,RoutedEventArgs e)
        {
            if (txtPasswordLama.Password.Length == 0||txtPasswordBaru.Password.Length==0)
            {
                var message = new MessageDialog("Pastikan semua field terisi");
                await message.ShowAsync();
            }
            else
            {
               
            }
        }
    }
}
