using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;


namespace SahabatSurabaya.Shared.Pages
{
    public sealed partial class Splashscreen : Page
    {
        HttpObject httpObject;
        Session session;
        DispatcherTimer timer;
        int time = 0;
        public Splashscreen()
        {
            this.InitializeComponent();
            httpObject = new HttpObject();
            session = new Session();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, object e)
        {
            time++;
            if (time == 2){
                timer.Stop();
                this.Frame.Navigate(typeof(LoginPage));
            }
        }

        private async void pageLoaded(object Sender,RoutedEventArgs e)
        {
            try
            {
                var secureStorage = await SecureStorage.GetAsync("jwt_token");
                if (secureStorage == null)
                {
                    timer.Start();
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
                        session.setTokenAuthorization(secureStorage.ToString());
                        if (userLogin.status_user == 2){
                            this.Frame.Navigate(typeof(HomeNavigationPageKepalaKeamanan));
                        }
                        else{
                            this.Frame.Navigate(typeof(HomeNavigationPage));
                        }
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
