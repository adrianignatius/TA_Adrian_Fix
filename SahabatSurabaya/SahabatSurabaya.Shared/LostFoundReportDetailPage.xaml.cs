using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
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

namespace SahabatSurabaya
{
    public sealed partial class LostFoundReportDetailPage : Page
    {
        string url = "ms-appx:///Assets/icon/";
        Session session;
        LostFoundReportParams param;
        HttpObject httpObject;
        User userLogin;
        public LostFoundReportDetailPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
        }
        

        public void pageLoaded(object sender,RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            param = session.getLostFoundReportParams();
            if (param.jenisLaporan == 0)
            {
                txtHeaderDetailLaporan.Text = "Konfirmasi Laporan Penemuan Barang";
                txtHeaderLokasi.Text = "Lokasi Penemuan";
            }
            else
            {
                txtHeaderDetailLaporan.Text = "Konfirmasi Laporan Kehilangan Barang";
                txtHeaderLokasi.Text = "Lokasi Penemuan";
            }
            imageIconBarang.Source = new BitmapImage(new Uri(url + param.namaFileGambar));
            txtJenisBarang.Text = param.displayJenisBarang;
            txtTanggalLaporan.Text = param.tglLaporan;
            txtJudulLaporan.Text = param.judulLaporan;
            txtDescBarang.Text = param.descLaporan;
            txtLokasiLaporan.Text = param.alamatLaporan;
        }
        public async void konfirmasi_laporan(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                //client.BaseAddress = new Uri(session.getApiURL());
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(param.judulLaporan), "judul_laporan");
                form.Add(new StringContent(param.jenisLaporan.ToString()), "jenis_laporan");
                form.Add(new StringContent(param.tglLaporan.ToString()), "tanggal_laporan");
                form.Add(new StringContent(param.waktuLaporan.ToString()), "waktu_laporan");
                form.Add(new StringContent(param.alamatLaporan), "alamat_laporan");
                form.Add(new StringContent(param.lat.ToString()), "lat_laporan");
                form.Add(new StringContent(param.lng.ToString()), "lng_laporan");
                form.Add(new StringContent(param.descLaporan), "deskripsi_barang");
                form.Add(new StringContent("0"), "status_laporan");
                form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pelapor");
                if (param.imageLaporan != null)
                {
                    form.Add(new StreamContent(new MemoryStream(param.imageLaporan.image)), "image", "image.jpg");
                }
                //for (int i = 0; i < param.listImage.Count; i++)
                //{
                //    form.Add(new StreamContent(new MemoryStream(param.listImage[i].image)), "image[]", "image.jpg"); ;
                //}
                HttpResponseMessage response = await client.PostAsync("insertLaporanLostFound", form);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(responseData);
                    var message = new MessageDialog(json["message"].ToString());
                    await message.ShowAsync();
                    if (json["status"].ToString() == "1")
                    {
                        this.Frame.Navigate(typeof(HomePage));
                    }

                }
            }
        }
    }
}
