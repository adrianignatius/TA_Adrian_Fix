using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace SahabatSurabaya.Shared.Pages
{
    public sealed partial class LaporanKepalaKeamananPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas;
        public LaporanKepalaKeamananPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            txtJudulHalaman.Text = "Daftar laporan di area kecamatan " + userLogin.kecamatan_user;
            loadLaporanKriminalitas();
        }

        private void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
            ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas", selected.thumbnail_gambar, selected.status_laporan, selected.jumlah_konfirmasi);
            session.setReportDetailPageParams(param);
            this.Frame.Navigate(typeof(ReportDetailPage));
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

        private async void loadLaporanKriminalitas()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getLaporanKriminalitas/" + userLogin.id_kecamatan_user, session.getTokenAuthorization());
            listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            if (listLaporanKriminalitas.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
            }
            lvLaporanArea.ItemsSource = listLaporanKriminalitas;
        }
    }
}
