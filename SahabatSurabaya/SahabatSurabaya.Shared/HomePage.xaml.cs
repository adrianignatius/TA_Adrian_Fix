using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Xamarin.Essentials;
#if __ANDROID__
using Com.OneSignal;
using Com.OneSignal.Abstractions;
#endif

namespace SahabatSurabaya
{

    public sealed partial class HomePage : Page
    {
        Session session;
        User userLogin;
        ObservableCollection<LaporanLostFound> listLaporanLostFound;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas;
        ObservableCollection<User> listEmergencyContact;
        DispatcherTimer timer;
        string lat="", lng = "", address = "";
        int time = 0;
        public HomePage()
        {
            this.InitializeComponent();
            listLaporanLostFound = new ObservableCollection<LaporanLostFound>();
            listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
            listEmergencyContact = new ObservableCollection<User>();
            session = new Session();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        private async void Timer_Tick(object sender, object e)
        {
            if (time == 2)
            {
                var asd = new MessageDialog("asd");
                await asd.ShowAsync();
                timer.Stop();
                time = 0;
            }
            else
            {
                time++;
            }
        }

        private async void getUserAddress()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }
            lat = location.Latitude.ToString().Replace(",", ".");
            lng = location.Longitude.ToString().Replace(",", ".");
            using (var client = new HttpClient())
            {
                string latlng = lat + "," + lng;
                string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
                HttpResponseMessage response = await client.GetAsync(reqUri);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(jsonString);
                    address = json["results"][0]["formatted_address"].ToString();
                }
            }
        }

        public async void HomePageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            txtNamaUser.Text = "Selamat Datang, " + userLogin.nama_user + "!";
            if (userLogin.status_user == 1)
            {
                txtStatusUser.Text = "Premium Account";
            }
            else
            {
                txtStatusUser.Text = "Free Account";
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getHeadlineLaporanKriminalitas");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
                    lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
                }
                else
                {
                    var message = new MessageDialog("asd");
                    await message.ShowAsync();
                }
                response = await client.GetAsync("/getHeadlineLaporanLostFound");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
                    lvLaporanLostFound.ItemsSource = listLaporanLostFound;
                }
            }
        }

        private void emergencyAction(object sender, HoldingRoutedEventArgs e)
        {
#if __ANDROID__
            if(e.HoldingState==HoldingState.Completed){
                getUserAddress();
                sendNotification();
            }
#endif
        }


        private async void sendEmergencyChat(User u)
        {
            string content = "Saya sedang dalam keadaan darurat! Lokasi terakhir saya di " + address;
            using (var client=new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("checkHeaderChat/" + userLogin.id_user+"/"+u.id_user);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(responseData);
                    var m = new MessageDialog(json["id_chat"].ToString());
                    await m.ShowAsync();    
                    //MultipartFormDataContent form = new MultipartFormDataContent();
                    //form.Add(new StringContent(responseData), "id_chat");
                    //form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pengirim");
                    //form.Add(new StringContent(u.id_user.ToString()), "id_user_penerima");
                    //form.Add(new StringContent(content), "isi_chat");
                    //response = await client.PostAsync("insertDetailChat/", form);
                }
            }
        }

#if __ANDROID__
        private async void sendNotification()
        {
            var m = new MessageDialog(address);
            await m.ShowAsync();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("user/getEmergencyContact/" + userLogin.id_user);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
                    foreach(User user in listEmergencyContact){
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("number", user.telpon_user),
                        });
                        response = await client.PostAsync("user/sendEmergencyNotification", content);
                        sendEmergencyChat(user);
                    }
                }
            }
        }
#endif

        public void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            LaporanLostFound selected = (LaporanLostFound)e.ClickedItem;
            ReportDetailPageParams param = new ReportDetailPageParams(userLogin, selected);
            session.setReportDetailPageParams(param);
            this.Frame.Navigate(typeof(ReportDetailPage));     
        }
    }
}
