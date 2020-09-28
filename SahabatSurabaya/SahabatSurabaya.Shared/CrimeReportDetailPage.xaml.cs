using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CrimeReportDetailPage : Page
    {
        Session session;
        CrimeReportParams param;
        User userLogin;
        string url = "ms-appx:///Assets/icon/";
        public CrimeReportDetailPage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        public void pageLoaded(object sender, RoutedEventArgs e)
        {
            param = session.getCrimeReportParams();
            userLogin = session.getUserLogin();
            txtJudulLaporan.Text = param.judulLaporan;
            txtJenisKejadian.Text = param.displayKategoriKejadian;
            txtTanggalLaporan.Text = param.tglLaporan;
            txtLokasiLaporan.Text = param.alamatLaporan;
            txtDescKejadian.Text = param.descLaporan;
            imageIconKejadian.Source = new BitmapImage(new Uri(url + param.namaFileGambar));
        }
        public async void konfirmasi_laporan(object sender,RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(param.judulLaporan), "judul_laporan");
                form.Add(new StringContent(param.displayKategoriKejadian), "jenis_kejadian");
                form.Add(new StringContent(param.descLaporan), "deskripsi_kejadian");
                form.Add(new StringContent(param.tglLaporan.ToString()), "tanggal_laporan");
                form.Add(new StringContent(param.waktuLaporan.ToString()), "waktu_laporan");
                form.Add(new StringContent(param.alamatLaporan), "alamat_laporan");
                form.Add(new StringContent(param.lat.ToString()), "lat_laporan");
                form.Add(new StringContent(param.lng.ToString()), "lng_laporan");
                form.Add(new StringContent("0"), "status_laporan");
                form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pelapor");
                //for (int i = 0; i < param.listImage.Count; i++)
                //{
                //    form.Add(new StreamContent(new MemoryStream(param.listImage[i].image)), "image[]", "image.jpg"); ;
                //}
                HttpResponseMessage response = await client.PostAsync("insertLaporanKriminalitas", form);
                if (response.IsSuccessStatusCode)
                {
                    var message = new MessageDialog("Berhasil membuat laporan!");
                    await message.ShowAsync();
                    this.Frame.Navigate(typeof(HomePage));
                }
                else
                {
                    var message = new MessageDialog("Gagal membuat laporan! Silahkan coba beberapa saat lagi");
                    await message.ShowAsync();
                }
            }
        }
    }
}
