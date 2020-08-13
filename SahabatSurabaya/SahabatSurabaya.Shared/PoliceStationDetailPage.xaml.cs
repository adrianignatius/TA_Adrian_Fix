using System;
using System.Collections.Generic;
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
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PoliceStationDetailPage : Page
    {
        KantorPolisi selected;
        public PoliceStationDetailPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            selected = e.Parameter as KantorPolisi;
            txtNamaKantorPolisi.Text = selected.nama_kantor_polisi;
            txtAlamatKantorPolisi.Text = selected.alamat_kantor_polisi;
            txtNoTelpKantorPolisi.Text = selected.notelp_kantor_polisi;       
        }

        private async void Back_Click(object sender, RoutedEventArgs e)
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

        private async void mapLoadCompleted(object sender, WebViewNavigationCompletedEventArgs e)
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }
            string[] args = { location.Latitude.ToString().Replace(",", "."), location.Longitude.ToString().Replace(",", "."), selected.lat_kantor_polisi, selected.lng_kantor_polisi };
            string lat = await webViewMap.InvokeScriptAsync("calculateRoute", args);
        }
    }

    
}
