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
using Windows.UI.Xaml.Navigation;

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

        private void pageLoaded(object sender,RoutedEventArgs e)
        {
            dtTanggalAwal.MaxYear = new DateTime(2023, 12, 31);
            dtTanggalAwal.MinYear = new DateTime(2020, 1, 31);
            dtTanggalAkhir.MaxYear = new DateTime(2023, 12, 31);
            dtTanggalAkhir.MinYear = new DateTime(2020, 1, 31);
            session.setFilterState(1);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private async void setFilter(object sender,RoutedEventArgs e)
        {
            if (validateInput() == true)
            {
                DateTime tanggal_awal = dtTanggalAwal.Date.DateTime;
                DateTime tanggal_akhir = dtTanggalAkhir.Date.DateTime;
                List<int> listIdKecamatanSelected = new List<int>();
                for (int i = 0; i < listKecamatanSelected.Count; i++)
                {
                    listIdKecamatanSelected.Add(listKecamatanSelected[i].id_kecamatan);
                }
                FilterParams param = new FilterParams(1, tanggal_awal.ToString("yyyy-MM-dd"), tanggal_akhir.ToString("yyyy-MM-dd"), listJenisLaporan, listIdBarangSelected, listIdKecamatanSelected);
                session.setFilterParams(param);
                this.Frame.Navigate(typeof(AllLostFoundReportPage));
                
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int? state = session.getFilterState();
            if (state == 0)
            {
                string responseData = await httpObject.GetRequest("settings/getKecamatan");
                listKecamatan = JsonConvert.DeserializeObject<ObservableCollection<Kecamatan>>(responseData);
                gvKecamatan.ItemsSource = listKecamatan;
                listKecamatanSelected.Clear();
                txtStackKecamatan.Text = "";
            }      
        }
    }
}
