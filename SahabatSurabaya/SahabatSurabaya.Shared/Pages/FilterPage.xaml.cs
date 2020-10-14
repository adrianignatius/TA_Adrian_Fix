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


namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class FilterPage : Page
    {
        static int mode;
        ObservableCollection<Kecamatan> listKecamatan;
        List<Kecamatan> listKecamatanSelected;
        List<int> listIdBarangSelected;
        List<int> listIdKejadianSelected;
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
            listIdKejadianSelected = new List<int>();
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
                if (mode == 0){
                    FilterParams param = new FilterParams(tanggal_awal.ToString("yyyy-MM-dd"), tanggal_akhir.ToString("yyyy-MM-dd"), listJenisLaporan, listIdBarangSelected, listIdKecamatanSelected);
                    session.setFilterParams(param);
                    this.Frame.Navigate(typeof(AllLostFoundReportPage));
                }
                else{
                    FilterParams param = new FilterParams(tanggal_awal.ToString("yyyy-MM-dd"), tanggal_akhir.ToString("yyyy-MM-dd"), null, listIdKejadianSelected, listIdKecamatanSelected);
                    session.setFilterParams(param);
                    this.Frame.Navigate(typeof(AllCrimeReportPage));
                }
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

        private void jenisKejadianChecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void jenisKejadianUnchecked(object sender, RoutedEventArgs e)
        {
            
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

        private void resetPage()
        {

            dtTanggalAwal.SelectedDate = null;
            dtTanggalAkhir.SelectedDate = null;
            cbBarang1.IsChecked = false;
            cbBarang2.IsChecked = false;
            cbBarang3.IsChecked = false;
            cbBarang4.IsChecked = false;
            cbBarang5.IsChecked = false;
            cbBarang6.IsChecked = false;
            cbBarang7.IsChecked = false;
            cbJenis1.IsChecked = false;
            cbJenis2.IsChecked = false;
            cbKejadian1.IsChecked = false;
            cbKejadian2.IsChecked = false;
            cbKejadian3.IsChecked = false;
            cbKejadian4.IsChecked = false;
            cbKejadian5.IsChecked = false;
            cbKejadian6.IsChecked = false;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int? state = session.getFilterState();
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(AllLostFoundReportPage)){
                stackKejadian.Visibility = Visibility.Collapsed;
                stackBarang.Visibility = Visibility.Visible;
                stackJenisLaporan.Visibility = Visibility.Visible;
                mode = 0;
            }
            else if(entry.SourcePageType == typeof(AllCrimeReportPage))
            {
                stackKejadian.Visibility = Visibility.Visible;
                stackBarang.Visibility = Visibility.Collapsed;
                stackJenisLaporan.Visibility = Visibility.Collapsed;
                mode = 1;
            }
            if (state == 0){
                resetPage();
                string responseData = await httpObject.GetRequest("settings/getKecamatan");
                listKecamatan = JsonConvert.DeserializeObject<ObservableCollection<Kecamatan>>(responseData);
                gvKecamatan.ItemsSource = listKecamatan;
                listKecamatanSelected.Clear();
                txtStackKecamatan.Text = "";
            }      
        }
    }
}
