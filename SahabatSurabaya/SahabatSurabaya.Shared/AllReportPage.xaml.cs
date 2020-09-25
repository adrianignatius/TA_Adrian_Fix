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
        ObservableCollection<LaporanLostFound> listLaporanLostFound = new ObservableCollection<LaporanLostFound>();
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
            loadHeadlineLaporanLostFound();
        }

        public void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            string tag = (sender as ListView).Tag.ToString();
            if (tag == "lvKriminalitas")
            {
                LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas");
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
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, jenis_laporan, selected.deskripsi_barang, selected.lat_laporan, selected.lng_laporan, "lostfound");
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

        private async void loadMoreData(object sender,RoutedEventArgs e)
        {

        }
    }
}
