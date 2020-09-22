using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SahabatSurabaya
{ 
    public sealed partial class AllReportPage : Page
    {
        Session session;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
        public AllReportPage()
        {
            this.InitializeComponent();
            session = new Session();
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

        private async void loadHeadlineLaporanKriminalitas()
        {
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
                    var message = new MessageDialog("Tidak ada koneksi internet, silahkan coba beberapa saat lagi");
                    await message.ShowAsync();
                }
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            string param = session.getAllReportParam();
            if (param == "kriminalitas")
            {
                rootPivotLaporan.SelectedItem = pvLaporanKriminalitas;
            }
            else
            {
                rootPivotLaporan.SelectedItem = pvLaporanLostFound;
            }
            loadHeadlineLaporanKriminalitas();
        }

        private async void loadMoreData(object sender,RoutedEventArgs e)
        {

        }
    }
}
