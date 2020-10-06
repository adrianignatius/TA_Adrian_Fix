using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared
{

    public sealed partial class HistoryPage : Page
    {
        Session session;
        User userLogin;
        HttpObject httpObject;
        ObservableCollection<LaporanLostFound> listHistoryLaporanLostFound;
        ObservableCollection<LaporanKriminalitas> listHistoryLaporanKriminalitas;
        public HistoryPage()
        {
            this.InitializeComponent();
            session = new Session();
            userLogin = session.getUserLogin();
            httpObject = new HttpObject();
            listHistoryLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
            listHistoryLaporanLostFound = new ObservableCollection<LaporanLostFound>();
        }

        private async void pageLoaded(object sender,RoutedEventArgs e)
        {
            btnSelectionLaporanLostFound.IsEnabled = false;
            string responseData = await httpObject.GetRequest("user/getHistoryLaporanLostFound/" + userLogin.id_user);
            listHistoryLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
            responseData = await httpObject.GetRequest("user/getHistoryLaporanKriminalitas/" + userLogin.id_user);
            listHistoryLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            lvHistory.ItemsSource = listHistoryLaporanLostFound;
        }

        private void changeSource(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            if (tag == "1")
            {
                lvHistory.ItemsSource = listHistoryLaporanLostFound;
                btnSelectionLaporanKriminalitas.IsEnabled = true;
                btnSelectionLaporanLostFound.IsEnabled = false;
                lvHistory.Tag = "lvLostfound";
            }
            else
            {
                lvHistory.ItemsSource = listHistoryLaporanKriminalitas;
                btnSelectionLaporanKriminalitas.IsEnabled = false;
                btnSelectionLaporanLostFound.IsEnabled = true;
                lvHistory.Tag = "lvKriminalitas";
            }
        }

    }
}
