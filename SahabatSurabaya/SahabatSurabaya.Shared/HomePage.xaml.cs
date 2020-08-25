using Newtonsoft.Json;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        User userLogin;
        ObservableCollection<LaporanLostFound> listLaporan;
        public HomePage()
        {
            this.InitializeComponent();
            listLaporan = new ObservableCollection<LaporanLostFound>();
        }
        public async void HomePageLoaded(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getHeadlineLaporanLostFound");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listLaporan = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
                    lvLaporanKriminalitas.ItemsSource = listLaporan;
                }
            }

        }

        public void goToDetailPage(object sender, RoutedEventArgs e)
        {
            int index = lvLaporanKriminalitas.SelectedIndex;
            this.Frame.Navigate(typeof(ReportDetailPage),listLaporan[index]);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            userLogin = e.Parameter as User;
            txtNamaUser.Text ="Selamat Datang,  "+userLogin.email_user+"!";
            if (userLogin.status_user == 1)
            {
                txtStatusUser.Text = "Premium Account";
            }
            else
            {
                txtStatusUser.Text = "Free Account";
            }
        }
    }
}
