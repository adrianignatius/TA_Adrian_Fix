using Newtonsoft.Json;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportDetailPage : Page
    {
        LaporanLostFound selected;
        ObservableCollection<KomentarLaporanLostFound> listKomentar;
        public ReportDetailPage()
        {          
            this.InitializeComponent();
        }

        private async void Back_Click(object sender, RoutedEventArgs e)
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            selected = e.Parameter as LaporanLostFound;
            txtNamaPengguna.Text = selected.email_pelapor;
            txtTanggalUpload.Text = selected.tanggal_laporan + " Pukul " + selected.waktu_laporan;
            txtDeskripsiLaporan.Text = selected.deskripsi_barang;
            txtJudulLaporan.Text = selected.judul_laporan;
            loadKomentarLaporan();
        }
        private async void mapLoadedCompleted(object sender, WebViewNavigationCompletedEventArgs e)
        {
            string[] args = {selected.lat_laporan,selected.lng_laporan };
            string lat = await webVieMapLokasi.InvokeScriptAsync("displayMap", args);
        }

        public async void loadKomentarLaporan()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getKomentarLaporanLostFound/"+selected.id_laporan);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listKomentar = JsonConvert.DeserializeObject<ObservableCollection<KomentarLaporanLostFound>>(responseData);
                    lvKomentarLaporan.ItemsSource = listKomentar;
                }
            }
        }
        public async void sendComment(object sender,RoutedEventArgs e)
        {
            if (txtKomentar.Text.Length != 0)
            {
                string isi_komentar = txtKomentar.Text;
                string tanggal_komentar = DateTime.Now.ToString("dd/MM/yyyy");
                string waktu_komentar = DateTime.Now.ToString("HH:mm:ss");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8080/");
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(selected.id_laporan), "id_laporan");
                    form.Add(new StringContent(isi_komentar), "isi_komentar");
                    form.Add(new StringContent(tanggal_komentar), "tanggal_komentar");
                    form.Add(new StringContent(waktu_komentar), "waktu_komentar");
                    form.Add(new StringContent("asd"), "email_user");
                    HttpResponseMessage response = await client.PostAsync("insertKomentarLaporanLostFound", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var message = new MessageDialog("Berhasil menambahkan komentar!");
                        await message.ShowAsync();
                        loadKomentarLaporan();
                    }
                }
            }
            else
            {
                var messageBox = new MessageDialog("Komentar yang dimasukkan tidak boleh kosong!");
                await messageBox.ShowAsync();
            }
        }
    }
}
