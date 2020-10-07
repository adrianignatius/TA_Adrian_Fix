using Newtonsoft.Json;
using SahabatSurabaya.Shared;
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
        HttpObject httpObject;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
        ObservableCollection<LaporanLostFound> listLaporanLostFound = new ObservableCollection<LaporanLostFound>();
        public AllReportPage()
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

        private async void loadLaporanLostFound()
        {
            if (listLaporanLostFound.Count == 0)
            {
                string responseData = await httpObject.GetRequest("getLaporanLostFound");
                listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
            }
            lvLaporan.ItemsSource = listLaporanLostFound;
        }

        private async void loadLaporanKriminalitas()
        {
            if (listLaporanKriminalitas.Count == 0)
            {
                string responseData = await httpObject.GetRequest("getLaporanKriminalitas");
                listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            }
            lvLaporan.ItemsSource = listLaporanKriminalitas;
        }

        private void setListViewLostFound()
        {
            btnSelectionLaporanLostFound.IsEnabled = false;
            btnSelectionLaporanKriminalitas.IsEnabled = true;
            loadLaporanLostFound();
            lvLaporan.Tag = "lvLostFound";
        }

        private void setListViewKriminalitas()
        {
            btnSelectionLaporanKriminalitas.IsEnabled = false;
            btnSelectionLaporanLostFound.IsEnabled = true;
            loadLaporanKriminalitas();
            lvLaporan.Tag = "lvKriminalitas";
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            string param = session.getAllReportParam();
            if (param == "kriminalitas")
            {
                setListViewKriminalitas();
            }
            else
            {
                setListViewLostFound();
            }
        }

        public void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            string tag = (sender as ListView).Tag.ToString();
            if (tag == "lvKriminalitas")
            {
                LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas",selected.thumbnail_gambar,selected.status_laporan);
                session.setReportDetailPageParams(param);
            }
            else if (tag == "lvLostFound")
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
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, jenis_laporan, selected.deskripsi_barang, selected.lat_laporan, selected.lng_laporan, "lostfound",selected.thumbnail_gambar,selected.status_laporan);
                session.setReportDetailPageParams(param);
            }
            this.Frame.Navigate(typeof(ReportDetailPage));
        }

        private void loadMoreData(object sender,RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FilterPage));
        }

        private void changeSource(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            if (tag == "1")
            {
                setListViewLostFound();
            }
            else
            {
                setListViewKriminalitas();
            }
        }
    }
}
