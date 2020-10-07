using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Essentials;

namespace SahabatSurabaya
{

    public sealed partial class ReportDetailPage : Page
    {
        ReportDetailPageParams param;
        User userLogin;
        ObservableCollection<KomentarLaporan> listKomentar;
        Session session;
        HttpObject httpObject;
        public ReportDetailPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            listKomentar = new ObservableCollection<KomentarLaporan>();
        }
        
        private void pageLoaded(object sender,RoutedEventArgs e)
        {
            param = session.getReportDetailPageParams();
            userLogin = session.getUserLogin();
            if (userLogin.id_user == param.id_user_pelapor)
            {
                btnChatPage.Visibility = Visibility.Collapsed;
                btnKonfirmasi.Visibility = Visibility.Collapsed;
            }
            if (param.status_laporan == 0)
            {
                txtStatusLaporan.Text = "Belum diverifikasi";
                txtStatusLaporan.Foreground = new SolidColorBrush(Colors.Red);
                btnShare.Visibility = Visibility.Collapsed;
                stackComment.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtStatusLaporan.Text = "Sudah diverifikasi";
                txtStatusLaporan.Foreground = new SolidColorBrush(Colors.Green);
            }
            string type = "";
            if (param.tag == "kriminalitas")
            {
                type = "2";
            }
            else
            {
                btnKonfirmasi.Visibility = Visibility.Collapsed;
                if(param.jenis_laporan == "Penemuan barang")
                {
                    type = "0";
                }
                else
                {
                    type = "1";
                }
            }
            imageLaporan.Source = new BitmapImage(new Uri(session.getUrlGambarLaporan() + param.thumbnail_gambar));
            txtNamaPengguna.Text = param.nama_user_pelapor;
            txtTanggalUpload.Text = param.tanggal_laporan + " Pukul " + param.waktu_laporan;
            txtDeskripsiLaporan.Text = param.deskripsi_laporan;
            txtJudulLaporan.Text = param.judul_laporan;
            txtAlamatLaporan.Text = param.alamat_laporan;
            txtJenisLaporan.Text = param.jenis_laporan;
            loadKomentarLaporan();
            webVieMapLokasi.Navigate(new Uri(session.getUrlWebView() + "location-map.php?lat=" + param.lat_laporan + "&lng=" + param.lng_laporan+"&type="+type));
        }

        private async void konfirmasiLaporan(object sender,RoutedEventArgs e)
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Apakah anda yakin ingin melakukan konfirmasi?",
                Content = "Dengan melakukan konfirmasi maka anda membantu meningkatkan kebenaran informasi laporan ini ",
                PrimaryButtonText = "Konfirmasi",
                CloseButtonText = "Batal"
            };
            ContentDialogResult result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                btnKonfirmasi.IsEnabled = false;
            }
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
        
        private async void openMap(object sender,RoutedEventArgs e) 
        {
            MapObject map = new MapObject();
            await map.openMapWithMarker(double.Parse(param.lat_laporan,CultureInfo.InvariantCulture), double.Parse(param.lng_laporan, CultureInfo.InvariantCulture), param.alamat_laporan);
        }


        public async void loadKomentarLaporan()
        {
            string responseData = await httpObject.GetRequest("getKomentarLaporan/" + param.id_laporan);
            listKomentar = JsonConvert.DeserializeObject<ObservableCollection<KomentarLaporan>>(responseData);
            lvKomentarLaporan.ItemsSource = listKomentar;
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    HttpResponseMessage response = await client.GetAsync("/getKomentarLaporan/"+ param.id_laporan);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = await response.Content.ReadAsStringAsync();
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        listKomentar = JsonConvert.DeserializeObject<ObservableCollection<KomentarLaporan>>(responseData);
            //        lvKomentarLaporan.ItemsSource = listKomentar;
            //    }
            //}
        }

        private async void shareLaporan(object sender, RoutedEventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = "asd",
                Title = "Share Text"
            });
        }
        private async void goToChatPage(object sender, RoutedEventArgs e)
        {
            string responseData = await httpObject.GetRequest("user/checkHeaderChat?id_user_1=" + userLogin.id_user + "&id_user_2=" + param.id_user_pelapor);
            JObject json = JObject.Parse(responseData);
            ChatPageParams chatParam = new ChatPageParams(Convert.ToInt32(json["id_chat"].ToString()), userLogin.id_user, param.id_user_pelapor, param.nama_user_pelapor);
            session.setChatPageParams(chatParam);
            this.Frame.Navigate(typeof(PersonalChatPage));
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    HttpResponseMessage response = await client.GetAsync("/checkHeaderChat/" + userLogin.id_user+"/"+param.id_user_pelapor);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        JObject json = JObject.Parse(responseData);
            //        ChatPageParams chatParam = new ChatPageParams(Convert.ToInt32(json["id_chat"].ToString()), userLogin.id_user, param.id_user_pelapor, param.nama_user_pelapor);
            //        session.setChatPageParams(chatParam);
            //        this.Frame.Navigate(typeof(PersonalChatPage));
            //    }
            //}
        }

        private async void sendComment(object sender,RoutedEventArgs e)
        {
            if (txtKomentar.Text.Length != 0)
            {
                string isi_komentar = txtKomentar.Text;
                string tanggal_komentar = DateTime.Now.ToString("dd/MM/yyyy");
                string waktu_komentar = DateTime.Now.ToString("HH:mm:ss");
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("id_laporan", param.id_laporan),
                    new KeyValuePair<string, string>("isi_komentar", isi_komentar),
                    new KeyValuePair<string, string>("tanggal_komentar", tanggal_komentar),
                    new KeyValuePair<string, string>("waktu_komentar", waktu_komentar),
                    new KeyValuePair<string, string>("id_user_komentar", userLogin.id_user.ToString())
                });
                string responseData = await httpObject.PostRequestWithUrlEncoded("user/insertKomentarLaporan", content);
                JObject json = JObject.Parse(responseData);
                var messageDialog = new MessageDialog(json["message"].ToString());
                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(session.getApiURL());
                //    MultipartFormDataContent form = new MultipartFormDataContent();
                //    form.Add(new StringContent(param.id_laporan), "id_laporan");
                //    form.Add(new StringContent(isi_komentar), "isi_komentar");
                //    form.Add(new StringContent(tanggal_komentar), "tanggal_komentar");
                //    form.Add(new StringContent(waktu_komentar), "waktu_komentar");
                //    form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_komentar");
                //    HttpResponseMessage response = await client.PostAsync("insertKomentarLaporan", form);
                //    if (response.IsSuccessStatusCode)
                //    {
                //        var message = new MessageDialog("Berhasil menambahkan komentar!");
                //        await message.ShowAsync();
                //        loadKomentarLaporan();
                //        txtKomentar.Text = "";
                //    }
                //}
            }
            else
            {
                var messageBox = new MessageDialog("Komentar yang dimasukkan tidak boleh kosong!");
                await messageBox.ShowAsync();
            }
        }
    }
}
