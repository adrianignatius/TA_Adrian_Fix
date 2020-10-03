using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SahabatSurabaya
{
    public sealed partial class ConfirmationProfileUpdatePage : Page
    {
        UpdateProfileParams param;
        Session session;
        HttpObject httpObject;
        public ConfirmationProfileUpdatePage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            param = e.Parameter as UpdateProfileParams;
            base.OnNavigatedTo(e);
            
        }

        private async void updateProfile(object sender, RoutedEventArgs e)
        {
            var formContent = new Dictionary<string, string>();
            formContent.Add("nama_user", param.nama_user);
            formContent.Add("id_user", param.id_user.ToString());
            formContent.Add("password_user", txtPassword.Password);
            if (param.lokasi_aktif_user == null) {
                formContent.Add("lokasi_available", "0");
            }
            else
            {
                formContent.Add("lokasi_available", "1");
                formContent.Add("lat_user", param.lat_user);
                formContent.Add("lng_user", param.lng_user);
                formContent.Add("lokasi_aktif_user", param.lokasi_aktif_user);
            }
            string responseData = await httpObject.PutRequest("user/updateProfile", new FormUrlEncodedContent(formContent));
            JObject json = JObject.Parse(responseData);
            var message = new MessageDialog(json["message"].ToString());
            await message.ShowAsync();
        }
    }
}
