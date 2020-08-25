using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http.Headers;
using Windows.UI.Popups;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
        }


        private void goToLogin(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));

        }

        private async void register(object sender, RoutedEventArgs e)
        {
            TextBox fullName =(TextBox) this.FindName("txtFullName");
            TextBox phoneNumber = (TextBox)this.FindName("txtPhone");
            PasswordBox password = (PasswordBox)this.FindName("txtPassword");
            TextBox email = (TextBox)(TextBox)this.FindName("txtEmail");
            if (fullName.Text.Length == 0 || phoneNumber.Text.Length == 0 || password.Password.Length == 0||email.Text.Length==0)
            {
                var dialog = new MessageDialog("Ada Field yang masih kosong!Silahkan lengkapi data terlebih dahulu!");
                await dialog.ShowAsync();
            }
            else
            {
                //using (var client = new HttpClient())
                //{
                //    User baru = new User(0, email.Text, fullName.Text, password.Password, phoneNumber.Text);
                //    client.BaseAddress = new Uri("http://localhost:8080/");
                //    client.DefaultRequestHeaders.Accept.Clear();
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    JObject oJsonObject = (JObject)JToken.FromObject(baru);
                //    var content = new StringContent(oJsonObject.ToString(), Encoding.UTF8, "application/json");
                ////    HttpResponseMessage response = await client.PostAsync(
                ////"user/insertUser/", new StringContent(oJsonObject.ToString(),Encoding.UTF8));
                //    var result = await client.PostAsync("user/insertUser", content);
                //    if (result.IsSuccessStatusCode)
                //    {
                //        var dialog = new MessageDialog("Register Berhasil!");
                //        await dialog.ShowAsync();
                //        fullName.Text = "";
                //        password.Password = "";
                //        phoneNumber.Text = "";
                //        email.Text = "";
                //    }


                //    //HttpResponseMessage response = await client.GetAsync("user/getAllUser/");
                //    //string jsonString = await response.Content.ReadAsStringAsync();
                //    //var dialog = new MessageDialog(jsonString);
                //    //await dialog.ShowAsync();
                //    //TextBox cb = (TextBox)this.FindName("txtFullName");
                //    //cb.Text = jsonString;
                //}
            }
            
                
            
        }
    }
}
