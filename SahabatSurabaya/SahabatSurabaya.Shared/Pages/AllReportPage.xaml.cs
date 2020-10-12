using Newtonsoft.Json;
using SahabatSurabaya.Shared;
using SahabatSurabaya.Shared.Class;
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
using Windows.UI.Xaml.Navigation;

namespace SahabatSurabaya.Shared.Pages
{ 
    public sealed partial class AllReportPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        ObservableCollection<LaporanLostFound> listLaporanLostFound = new ObservableCollection<LaporanLostFound>();
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
        public AllReportPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            userLogin = session.getUserLogin();
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

        private void goToFilterPage(object sender,RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FilterPage));
        }

        private async void loadLaporanLostFound()
        {
            if (listLaporanLostFound.Count == 0){
                string responseData = await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getLaporanLostFound/" + userLogin.kecamatan_user, session.getTokenAuthorization());
                //string responseData = userLogin.status_user == 2 ? await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getLaporanLostFound/" + userLogin.kecamatan_user, session.getTokenAuthorization()) : await httpObject.GetRequestWithAuthorization("getLaporanLostFound", session.getTokenAuthorization());
                listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
            }
            if (listLaporanLostFound.Count == 0){
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
            }
            lvLaporan.ItemsSource = listLaporanLostFound;
        }

        private async void loadLaporanKriminalitas()
        {
            if (listLaporanKriminalitas.Count == 0){
                string responseData = await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getLaporanKriminalitas/" + userLogin.kecamatan_user, session.getTokenAuthorization());
                //string responseData = userLogin.status_user == 2 ? await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getLaporanKriminalitas/" + userLogin.kecamatan_user, session.getTokenAuthorization()) : await httpObject.GetRequestWithAuthorization("getLaporanKriminalitas", session.getTokenAuthorization());
                listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            }
            if (listLaporanKriminalitas.Count == 0){
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
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
            txtJudulHalaman.Text = "Daftar laporan di area kecamatan " + userLogin.kecamatan_user;
            string param = session.getAllReportParam();
            if (param == "kriminalitas"){
                setListViewKriminalitas();
            }
            else{
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
                string jenis_laporan = selected.jenis_laporan == 0 ? "Penemuan " + selected.jenis_barang : "Kehilangan " + selected.jenis_barang;
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(FilterPage))
            {
                FilterParams param = e.Parameter as FilterParams;
                if (param.status_cari == 0)
                {
                    var message = new MessageDialog("batal");
                    await message.ShowAsync();
                }
                else
                {
                    var message = new MessageDialog("cari");
                    await message.ShowAsync();
                }
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
            }
        }
    }
}
