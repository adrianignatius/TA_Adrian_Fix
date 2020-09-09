using SahabatSurabaya.Shared;
using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Essentials;

namespace SahabatSurabaya
{

    public sealed partial class PoliceStationDetailPage : Page
    {
        KantorPolisi selected;
        Session session;
        MapObject map;
        public PoliceStationDetailPage()
        {
            this.InitializeComponent();
            session = new Session();
        }

        public void pageLoaded(object sender, RoutedEventArgs e)
        {
            selected = session.getKantorPolisi();
            txtNamaKantorPolisi.Text = selected.nama_kantor_polisi;
            txtAlamatKantorPolisi.Text = selected.alamat_kantor_polisi;
            txtNoTelpKantorPolisi.Text = selected.notelp_kantor_polisi;
            imageKantorPolisi.Source = new BitmapImage(new Uri(session.getUrlAssets() + selected.nama_file_gambar));
            loadMap();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();     
        }
        private async void openMap(object sender,RoutedEventArgs e)
        {
            map = new MapObject();
            await map.navigateToLocation(double.Parse(selected.lat_kantor_polisi, CultureInfo.InvariantCulture), double.Parse(selected.lng_kantor_polisi, CultureInfo.InvariantCulture));
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

        private async void loadMap()
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
            string latOrigin = location.Latitude.ToString().Replace(",", ".");
            string lngOrigin = location.Longitude.ToString().Replace(",", ".");
            string latDest = selected.lat_kantor_polisi;
            string lngDest = selected.lng_kantor_polisi;
            string name = selected.nama_kantor_polisi;
            webViewMap.Navigate(new Uri(session.getUrlWebView() + "location-kantor-polisi.php?latOrigin=" + latOrigin + "&lngOrigin=" + lngOrigin + "&latDest=" + latDest + "&lngDest=" + lngDest + "&name=" + name));
        }
    }

    
}
