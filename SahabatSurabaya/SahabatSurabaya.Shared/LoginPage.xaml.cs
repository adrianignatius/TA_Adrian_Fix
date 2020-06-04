using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        public void goToRegister(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }
        
        public async void login(object sender, RoutedEventArgs e)
        {
            TextBox email = (TextBox)this.FindName("txtEmail");
            PasswordBox password = (PasswordBox)this.FindName("txtPassword");
            if (email.Text.Length == 0 || password.Password.Length == 0)
            {
                var dialog = new MessageDialog("Silahkan isi Email dan Password terlebih dahulu!");
                await dialog.ShowAsync();
            }
            else
            {
                using (var client = new HttpClient())
                {                  
                    client.BaseAddress = new Uri("http://localhost:8080/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(email.Text), "email");
                    form.Add(new StringContent(password.Password), "password");
                    HttpResponseMessage response = await client.PostAsync("user/checkLogin", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        JObject json = JObject.Parse(responseData);
                        string checkLogin = json["data"].ToString();
                        if (checkLogin == "1")
                        {
                            this.Frame.Navigate(typeof(HomeNavigationPage));
                        }
                        else
                        {
                            var dialog = new MessageDialog("Username/Password salah!");
                            await dialog.ShowAsync();
                        }
                    }

                    //    //var jsonString = await response.Content.ReadAsStringAsync();

                    //    //string data = json["data"][0].ToString();
                    //    //User user = JsonConvert.DeserializeObject<User>(data);

                    //    //string a = json["data"][0]["email_user"].ToString();
                    //    //var dialog = new MessageDialog(user.email_user.ToString());
                    //    //await dialog.ShowAsync();
                    //    //var result = JsonConvert.DeserializeObject<ApiDataUser>(responseData);
                    //    //var jsonString = await response.Content.ReadAsStringAsync();
                    //    //JObject json = JObject.Parse(jsonString);
                    //    //string data = json["data"][0].ToString();
                    //    //User user = JsonConvert.DeserializeObject<User>(data);
                    //    ////string a = json["status"][0]["ada"].ToString();
                    //    //string a = json["data"][0]["email_user"].ToString();

                    //    if (checkLogin == "1")
                    //    {
                    //        this.Frame.Navigate(typeof(HomeNavigationPage));
                    //    }
                    //    else
                    //    {
                    //        var dialog = new MessageDialog("Username/Password salah!");
                    //        await dialog.ShowAsync();
                    //    }                       
                    //}

                }
            }
        }
    }
}
