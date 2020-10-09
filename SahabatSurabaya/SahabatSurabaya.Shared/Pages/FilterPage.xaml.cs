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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class FilterPage : Page
    {
        ObservableCollection<Kecamatan> listKecamatan;
        List<string> listKecamatanSelected;
        Session session;
        HttpObject httpObject;
        public FilterPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            listKecamatan = new ObservableCollection<Kecamatan>();
            listKecamatanSelected = new List<string>();
        }

        private async void pageLoaded(object sender,RoutedEventArgs e)
        {
            string responseData = await httpObject.GetRequest("admin/getKecamatan",session.getTokenAuthorization());
            listKecamatan = JsonConvert.DeserializeObject<ObservableCollection<Kecamatan>>(responseData);
            gvKecamatan.ItemsSource = listKecamatan;
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

        private void flyoutDone(object sender,RoutedEventArgs e)
        {
            flyoutKecamatan.Hide();
            int count = listKecamatanSelected.Count;
            string kecamatan = "";
            for (int i = 0; i < count; i++)
            {
                kecamatan += listKecamatanSelected[i] + ",";
            }
            txtStackKecamatan.Text = kecamatan.Substring(0,kecamatan.Length-1);
        }

        private void kecamatanUnchecked(object sender, RoutedEventArgs e)
        {
            string kecamatan = (sender as CheckBox).Content.ToString();
            listKecamatanSelected.Remove(kecamatan);
        }

        private void kecamatanChecked(object sender,RoutedEventArgs e)
        {
            string kecamatan = (sender as CheckBox).Content.ToString() ;
            listKecamatanSelected.Add(kecamatan);
        }
    }
}
