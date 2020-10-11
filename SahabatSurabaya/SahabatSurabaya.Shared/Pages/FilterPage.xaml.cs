using Newtonsoft.Json;
using SahabatSurabaya.Shared.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        Session session;
        HttpObject httpObject;
        public FilterPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            listKecamatan = new ObservableCollection<Kecamatan>();
            listIdBarangSelected = new List<int>();
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

        private async void setFilter(object sender,RoutedEventArgs e)
        {
            if(dtTanggalAwal.SelectedDate==null || dtTanggalAkhir.SelectedDate == null)
            {
                DateTime tanggal_awal = dtTanggalAwal.Date.DateTime;
                DateTime tanggal_akhir = dtTanggalAkhir.Date.DateTime;
                if (DateTime.Compare(tanggal_awal, tanggal_akhir) > 0){
                    var message = new MessageDialog("Tanggal awal tidak boleh lebih kecil dari tanggal akhir");
                    await message.ShowAsync();
                }
                FilterParams param=new FilterParams(tanggal_awal.ToString("yyyy-MM-dd"), tanggal_akhir.ToString("yyyy-MM-dd"),listIdBarangSelected,listkeca)
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
