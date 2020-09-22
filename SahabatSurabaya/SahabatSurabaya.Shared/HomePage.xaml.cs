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

        private void goToAllReportPage(object sender,RoutedEventArgs e)
        {
            string tag = (sender as TextBlock).Tag.ToString();
            session.setAllReportParam(tag);
            this.Frame.Navigate(typeof(AllReportPage));
        }

        private async void loadHeadlineLaporanKriminalitas()
        {
            using (var client=new HttpClient())
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
                    var message = new MessageDialog("Tidak ada koneksi internet, silahkan coba beberapa saat lagi");
                    await message.ShowAsync();
                }
            }
        }

        private async void loadHeadlineLaporanLostFound()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getHeadlineLaporanLostFound");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
                    lvLaporanLostFound.ItemsSource = listLaporanLostFound;

                }
                else
                {
                    var message = new MessageDialog("Tidak ada koneksi internet, silahkan coba beberapa saat lagi");
                    await message.ShowAsync();
                }
            }
        }

        public void HomePageLoaded(object sender, RoutedEventArgs e)
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
            loadHeadlineLaporanKriminalitas();
            loadHeadlineLaporanLostFound();
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

        public async void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            string tag = (sender as ListView).Tag.ToString();
            if (tag == "lvKriminalitas")
            {
                LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan,selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan,selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas");
                session.setReportDetailPageParams(param);
            }
            else if (tag == "lvLostfound")
            {
                LaporanLostFound selected = (LaporanLostFound)e.ClickedItem;
                string jenis_laporan = "";
                if (selected.jenis_laporan == 0)
                {
                    jenis_laporan = "Penemuan barang";
                }
                else
                {
                    jenis_laporan = "Kehilangan barang";
                }
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan,jenis_laporan, selected.deskripsi_barang, selected.lat_laporan, selected.lng_laporan, "lostfound");
                session.setReportDetailPageParams(param);
            }
            //var contentPresenter = this.Frame.Parent as ContentPresenter;
            //var a=this.Frame.pa
            //var grid = contentPresenter.Parent as Grid;
            //var parent = grid.Parent;
            //var message = new MessageDialog(parent.ToString());
            //await message.ShowAsync();
            //var parent = (NavigationView.Parent as Grid).Parent as HomeNavigationPage;
            //parent.Frame.Navigate(typeof(ReportDetailPage));  
            this.Frame.Navigate(typeof(ReportDetailPage));
        }
    }
}
