using Newtonsoft.Json;
using SahabatSurabaya.Shared.Class;
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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace SahabatSurabaya.Shared.Pages
{
    
    public sealed partial class AllCrimeReportPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
        public AllCrimeReportPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            userLogin = session.getUserLogin();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            loadLaporanKriminalitas();
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
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/getLaporanKriminalitas", session.getTokenAuthorization());
            listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            if (listLaporanKriminalitas.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
            }
            lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
        }

        private void goToFilterPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FilterPage));
        }

        private void showLoading()
        {
            stackLoading.Visibility = Visibility.Visible;
            svListView.Visibility = Visibility.Collapsed;
        }

        private void hideLoading()
        {
            stackLoading.Visibility = Visibility.Collapsed;
            svListView.Visibility = Visibility.Visible;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(FilterPage))
            {
                showLoading();
                FilterParams param = session.getFilterParams();
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                string reqUri = "laporan/getLaporanLostFoundWithFilter?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&jenis_laporan=" + param.getArrayJenisLaporan() + "&id_barang=" + param.getArrayIdBarang() + "&id_kecamatan=" + param.getArrayIdKecamatan();
                string responseData = await httpObject.GetRequestWithAuthorization(reqUri, session.getTokenAuthorization());
                listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
                if (listLaporanKriminalitas.Count == 0)
                {
                    stackEmpty.Visibility = Visibility.Visible;
                    svListView.Visibility = Visibility.Collapsed;
                    stackLoading.Visibility = Visibility.Collapsed;
                    txtEmptyState.Text = "Tidak ada laporan yang sesuai dengan kriteria pencarian";
                }
                else
                {
                    hideLoading();
                    lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
                }
            }
            else
            {
                loadLaporanKriminalitas();
            }
        }
    }
}
