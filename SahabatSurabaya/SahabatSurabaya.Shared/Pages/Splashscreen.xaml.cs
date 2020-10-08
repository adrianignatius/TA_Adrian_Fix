using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;


namespace SahabatSurabaya.Shared
{
    public sealed partial class Splashscreen : Page
    {
        HttpObject httpObject;
        Session session;
        public Splashscreen()
        {
            this.InitializeComponent();
            httpObject = new HttpObject();
            session = new Session();
        }

        private async void pageLoaded(object Sender,RoutedEventArgs e)
        {
            try
            {
                var secureStorage = await SecureStorage.GetAsync("jwt_token");
                if (secureStorage == null)
                {
                    this.Frame.Navigate(typeof(LoginPage));
                }
                else
                {
                    var content = new FormUrlEncodedContent(new[]{
                        new KeyValuePair<string, string>("token", secureStorage.ToString())
                    });
                    string responseData = await httpObject.PostRequestWithUrlEncoded("sessionSignIn",content);
                    JObject json = JObject.Parse(responseData);
                    if (json["status"].ToString() == "1")
                    {
                        string data = json["data"].ToString();
                        User userLogin = JsonConvert.DeserializeObject<User>(data);
                        session.setUserLogin(userLogin);
                        this.Frame.Navigate(typeof(HomeNavigationPage));
                    }
                    else
                    {
                        SecureStorage.Remove("jwt_token");
                        this.Frame.Navigate(typeof(LoginPage));
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
