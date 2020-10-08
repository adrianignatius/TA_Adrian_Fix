using Newtonsoft.Json.Linq;
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
        HttpObject httpObject;
        public ChangePasswordPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
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
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("id_user", userLogin.id_user.ToString()),
                    new KeyValuePair<string, string>("old_password", txtPasswordLama.Password),
                    new KeyValuePair<string, string>("new_password", txtPasswordBaru.Password)
                });
                string responseData = await httpObject.PutRequest("user/changePassword",content);
                JObject json = JObject.Parse(responseData);
                var message = new MessageDialog(json["message"].ToString());
                await message.ShowAsync();
                if (json["status"].ToString() == "1")
                {
                    this.Frame.GoBack();
                }
                else
                {
                    txtPasswordBaru.Password = "";
                    txtPasswordLama.Password = "";
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }
    }
}
