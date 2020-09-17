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
                sendNotification();
            }
#endif
        }


#if __ANDROID__
        private async void sendNotification()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("user/getEmergencyContact/" + userLogin.id_user);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
                    foreach(User u in listEmergencyContact){
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("number", u.telpon_user),
                        });
                        response = await client.PostAsync("user/sendEmergencyNotification", content);
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
