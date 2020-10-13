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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllLostFoundReportPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        ObservableCollection<LaporanLostFound> listLaporanLostFound = new ObservableCollection<LaporanLostFound>();
        public AllLostFoundReportPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            userLogin = session.getUserLogin();
        }

        private void pageLoaded(object sender,RoutedEventArgs e)
        {
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

        private async void loadLaporanLostFound()
        {   
            string responseData = await httpObject.GetRequestWithAuthorization("laporan/getLaporanLostFound", session.getTokenAuthorization());
            listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
            if (listLaporanLostFound.Count == 0)
            {
                stackEmpty.Visibility = Visibility.Visible;
                svListView.Visibility = Visibility.Collapsed;
            }
            lvLaporanLostFound.ItemsSource = listLaporanLostFound;
        }

        private void goToFilterPage(object sender,RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FilterPage));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(FilterPage))
            {
                FilterParams param = session.getFilterParams();
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                this.Frame.BackStack.RemoveAt(this.Frame.BackStackDepth - 1);
                string reqUri = "laporan/getLaporanLostFoundWithFilter?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&jenis_laporan=" + param.getArrayJenisLaporan() + "&id_barang=" + param.getArrayIdBarang() + "&id_kecamatan=" + param.getArrayIdKecamatan();
                string responseData = await httpObject.GetRequestWithAuthorization("laporan/getLaporanLostFoundWithFilter", session.getTokenAuthorization());
                listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
                if (listLaporanLostFound.Count == 0)
                {
                    stackEmpty.Visibility = Visibility.Visible;
                    svListView.Visibility = Visibility.Collapsed;
                }
                lvLaporanLostFound.ItemsSource = listLaporanLostFound;
            }
            else
            {
                loadLaporanLostFound();
            }
        }
    }
}
