using Newtonsoft.Json;
using SahabatSurabaya.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class FilterPage : Page
    {
        ObservableCollection<Kecamatan> listKecamatan;
        List<Kecamatan> listKecamatanSelected;
        List<int> listIdBarangSelected;
        List<int> listJenisLaporan;
        Session session;
        HttpObject httpObject;
        public FilterPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            listKecamatan = new ObservableCollection<Kecamatan>();
            listIdBarangSelected = new List<int>();
            listJenisLaporan = new List<int>();
            listKecamatanSelected = new List<Kecamatan>();
        }

        private async void pageLoaded(object sender,RoutedEventArgs e)
        {
            string responseData = await httpObject.GetRequest("getKecamatan");
            listKecamatan = JsonConvert.DeserializeObject<ObservableCollection<Kecamatan>>(responseData);
            gvKecamatan.ItemsSource = listKecamatan;
            dtTanggalAwal.MaxYear = new DateTime(2023, 12, 31);
            dtTanggalAwal.MinYear = new DateTime(2020, 1, 31);
            dtTanggalAkhir.MaxYear = new DateTime(2023, 12, 31);
            dtTanggalAkhir.MinYear = new DateTime(2020, 1, 31);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            FilterParams param = new FilterParams(0, null,null,null,null,null);
            this.Frame.Navigate(typeof(AllReportPage),param);
        }

        private async void setFilter(object sender,RoutedEventArgs e)
        {
            if (validateInput() == true)
            {
                var messagesd = new MessageDialog(listJenisLaporan.Count.ToString());
                await messagesd.ShowAsync();
                DateTime tanggal_awal = dtTanggalAwal.Date.DateTime;
                DateTime tanggal_akhir = dtTanggalAkhir.Date.DateTime;
                List<int> listIdKecamatanSelected = new List<int>();
                for (int i = 0; i < listKecamatanSelected.Count; i++)
                {
                    listIdKecamatanSelected.Add(listKecamatanSelected[i].id_kecamatan);
                }
                FilterParams param = new FilterParams(1,tanggal_awal.ToString("yyyy-MM-dd"), tanggal_akhir.ToString("yyyy-MM-dd"), listJenisLaporan, listIdBarangSelected, listIdKecamatanSelected);
                this.Frame.Navigate(typeof(AllReportPage), param);
                //string json = JsonConvert.SerializeObject(param, Formatting.Indented);
                //using (var client = new HttpClient())
                //{
                //    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                //    client.BaseAddress = new Uri("http://localhost:8080/");
                //    HttpResponseMessage result = await client.PostAsync("getLaporanLostFoundWithFilter", content);
                //    if (result.IsSuccessStatusCode)
                //    {
                //        var responseData = result.Content.ReadAsStringAsync().Result;
                //        var message = new MessageDialog(responseData);
                //        await message.ShowAsync();
                //    }

                //}
            }
            else
            {
                var messageDialog = new MessageDialog("Harap isi semua kriteria yang dibutuhkan untuk pencarian");
                await messageDialog.ShowAsync();    
            }
        }
        private bool validateInput()
        {
            if(dtTanggalAwal.SelectedDate == null || dtTanggalAkhir.SelectedDate == null)
            {
                return false;
            }
            else
            {
                if (listJenisLaporan.Count == 0 || listIdBarangSelected.Count==0||listKecamatanSelected.Count==0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            
        }

        private void flyoutDone(object sender,RoutedEventArgs e)
        {
            flyoutKecamatan.Hide();
            if (listKecamatanSelected.Count > 0)
            {
                string kecamatan = "";
                for (int i = 0; i < listKecamatanSelected.Count; i++)
                {
                    kecamatan += listKecamatanSelected[i].nama_kecamatan + ",";
                }
                txtStackKecamatan.Text = kecamatan.Substring(0, kecamatan.Length - 1);
            }
        }

        private void jenisLaporanChecked(object sender, RoutedEventArgs e)
        {
            int id_jenis_laporan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listJenisLaporan.Add(id_jenis_laporan);
        }

        private void jenisLaporanUnchecked(object sender, RoutedEventArgs e)
        {
            int id_jenis_laporan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listJenisLaporan.Remove(id_jenis_laporan);
        }

        private void jenisBarangChecked(object sender, RoutedEventArgs e)
        {
            int id_barang = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listIdBarangSelected.Add(id_barang);
        }

        private void jenisBarangUnchecked(object sender, RoutedEventArgs e)
        {
            int id_barang = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            listIdBarangSelected.Remove(id_barang);
        }

        private void kecamatanUnchecked(object sender, RoutedEventArgs e)
        {
            int id_kecamatan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            Kecamatan selected = listKecamatan.Single(i => i.id_kecamatan == id_kecamatan);
            listKecamatanSelected.Remove(selected);
        }

        private void kecamatanChecked(object sender,RoutedEventArgs e)
        {
            int id_kecamatan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            Kecamatan selected = listKecamatan.Single(i => i.id_kecamatan == id_kecamatan);
            listKecamatanSelected.Add(selected);
        }
    }
}
