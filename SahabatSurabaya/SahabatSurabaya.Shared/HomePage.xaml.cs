using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
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
            
            
            

#if NETFX_CORE
            btnEmergency.Visibility=Visibility.Collapsed;
#endif
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

        private async Task<string> getUserAddress()
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
            string lat = location.Latitude.ToString().Replace(",", ".");
            string lng = location.Longitude.ToString().Replace(",", ".");
            using (var client = new HttpClient())
            {
                string latlng = lat + "," + lng;
                string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
                HttpResponseMessage response = await client.GetAsync(reqUri);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(jsonString);
                    string address = json["results"][0]["formatted_address"].ToString();
                    return address;
                }
                else
                {
                    return null;
                }
            }
        }

        public async void HomePageLoaded(object sender, RoutedEventArgs e)
        {
            //btnNext.SetValue(Canvas.LeftProperty, canvasWidth);
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

        private void nextContent(object sender, RoutedEventArgs e)
        {
            lvLaporanKriminalitas.ScrollIntoView(listLaporanKriminalitas[3]);
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


        private async void sendEmergencyChat(User u, string address)
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
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(json["id_chat"].ToString()), "id_chat");
                    form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pengirim");
                    form.Add(new StringContent(u.id_user.ToString()), "id_user_penerima");
                    form.Add(new StringContent(content), "isi_chat");
                    response = await client.PostAsync("insertDetailChat", form);
                    if (response.IsSuccessStatusCode)
                    {
                        
                    }
                }
            }
        }

#if __ANDROID__
        private async void sendNotification()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                string address = await getUserAddress();
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
                        sendEmergencyChat(user,address);
                    }
                    var message = new MessageDialog("Pesan darurat telah dikirimkan ke semua kontak darurat anda");
                    await message.ShowAsync();
                }
            }
        }
#endif

        public void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            string tag = (sender as ListView).Tag.ToString();
            ReportDetailPageParams param=new ReportDetailPageParams(null,null,null);
            if (tag == "lvKriminalitas")
            {
                LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
                param = new ReportDetailPageParams(null, selected, "kriminalitas");
            }
            else if (tag == "lvLostfound")
            {
                LaporanLostFound selected = (LaporanLostFound)e.ClickedItem;
                param = new ReportDetailPageParams(selected, null, "lostfound");
            }
            session.setReportDetailPageParams(param);
            this.Frame.Navigate(typeof(ReportDetailPage));     
        }
    }
}
