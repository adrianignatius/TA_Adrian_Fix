using Newtonsoft.Json;
using SahabatSurabaya.Shared.Class;
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
            session.setFilterState(null);
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
            if (session.getFilterState() == null)
            {
                session.setFilterState(0);
            }
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
                var message = new MessageDialog(param.getArrayIdKategori());
                await message.ShowAsync();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://adrian-webservice.ta-istts.com/");
                    client.DefaultRequestHeaders.Add("Authorization", session.getTokenAuthorization());
                    string reqUri = "laporan/getLaporanKriminalitasWithFilter?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&id_kejadian=" + param.getArrayIdKategori() + "&id_kecamatan=" + param.getArrayIdKecamatan();
                    HttpResponseMessage response = await client.GetAsync(reqUri);
                    if (response.IsSuccessStatusCode)
                    {
                        var m = new MessageDialog(response.Content.ReadAsStringAsync().Result);
                        await m.ShowAsync();
                    }
                }
                //string reqUri = "laporan/getLaporanKriminalitasWithFilter?tanggal_awal=" + param.tanggal_awal + "&tanggal_akhir=" + param.tanggal_akhir + "&id_kejadian=" + param.getArrayIdKategori() + "&id_kecamatan=" + param.getArrayIdKecamatan();
                //string responseData = await httpObject.GetRequestWithAuthorization(reqUri, session.getTokenAuthorization());
                //listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
                //if (listLaporanKriminalitas.Count == 0)
                //{
                //    stackEmpty.Visibility = Visibility.Visible;
                //    svListView.Visibility = Visibility.Collapsed;
                //    stackLoading.Visibility = Visibility.Collapsed;
                //    txtEmptyState.Text = "Tidak ada laporan yang sesuai dengan kriteria pencarian";
                //}
                //else
                //{
                //    hideLoading();
                //    lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
                //}
            }
            else
            {
                loadLaporanKriminalitas();
            }
        }
    }
}
