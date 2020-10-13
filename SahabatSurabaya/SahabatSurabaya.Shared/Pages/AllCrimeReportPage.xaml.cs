using Newtonsoft.Json;
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
            txtJudulHalaman.Text = userLogin.status_user == 2 ? "Daftar Laporan Kriminalitas di Area " + userLogin.kecamatan_user : "Daftar Laporan Kriminalitas";
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
            string responseData = userLogin.status_user == 2 ? await httpObject.GetRequestWithAuthorization("kepalaKeamanan/getLaporanKriminalitas/" + userLogin.id_kecamatan_user, session.getTokenAuthorization()) : await httpObject.GetRequestWithAuthorization("laporan/getLaporanKriminalitas", session.getTokenAuthorization());
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
    }
}
