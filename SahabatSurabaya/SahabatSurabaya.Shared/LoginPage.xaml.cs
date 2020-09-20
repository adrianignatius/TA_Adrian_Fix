using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#if __ANDROID__
using Com.OneSignal;
using Com.OneSignal.Abstractions;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    public sealed partial class LoginPage : Page
    {
        Session session;
        public LoginPage()
        {
            this.InitializeComponent();
            session = new Session();          
        }

        public void goToRegister(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }
        
        public async void login(object sender, RoutedEventArgs e)
        {

            if (txtEmail.Text.Length == 0 || txtPassword.Password.Length == 0)
            {
                var dialog = new MessageDialog("Silahkan isi Email dan Password terlebih dahulu!");
                await dialog.ShowAsync();
            }
            else
            {
                using (var client = new HttpClient())
                {                  
                    client.BaseAddress = new Uri(session.getApiURL());
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(txtEmail.Text), "email");
                    form.Add(new StringContent(txtPassword.Password), "password");
                    HttpResponseMessage response = await client.PostAsync("user/checkLogin", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        JObject json = JObject.Parse(responseData);
                        string statusCode = json["status"].ToString();
                        if (statusCode == "200")
                        {
                            string data = json["data"].ToString();
                            User userLogin= JsonConvert.DeserializeObject<User>(data);
                            session.setUserLogin(userLogin);
                            if (userLogin.status_user == 99)
                            {
                                this.Frame.Navigate(typeof(VerifyOtpPage));
                            }
                            else
                            {
                                this.Frame.Navigate(typeof(HomeNavigationPage));
                            }                          
                            
#if __ANDROID__
                              OneSignal.Current.SendTags(new Dictionary<string, string>() { {"no_handphone", userLogin.telpon_user}, {"tipe_user", userLogin.status_user.ToString()} });               
#endif
                        }
                        else
                        {
                            var dialog = new MessageDialog(json["message"].ToString());
                            await dialog.ShowAsync();
                        }
                    }

                }
            }
        }
    }
}
