using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class PoliceStationPage : Page
    {
        ObservableCollection<KantorPolisi> listKantorPolisi;
        Session session;
        HttpObject httpObject;
        public PoliceStationPage()
        {
            this.InitializeComponent();
            httpObject = new HttpObject();
        }
        public async void PoliceStationPageLoaded(object sender,RoutedEventArgs e)
        {
            session = new Session();
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(30)
                }); 
            }
            string origins = location.Latitude.ToString().Replace(",",".") + "," + location.Longitude.ToString().Replace(",", ".");
            string responseData = await httpObject.GetRequestWithAuthorization("settings/getAllKantorPolisi", session.getTokenAuthorization());
            string destinations = "";
            listKantorPolisi = JsonConvert.DeserializeObject<ObservableCollection<KantorPolisi>>(responseData);
            for (int i = 0; i < listKantorPolisi.Count; i++)
            {
                destinations += listKantorPolisi[i].lat_kantor_polisi.Replace(",", ".") + "," + listKantorPolisi[i].lng_kantor_polisi.Replace(",", ".") + "|";
            }
            destinations.Remove(destinations.Length - 1, 1);
            string reqUri = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metrics&origins=" + origins + "&destinations=" + destinations + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            responseData = await httpObject.GetRequest(reqUri);
            JObject json = JObject.Parse(responseData);
            var token = JToken.Parse(responseData)["rows"][0]["elements"].ToList().Count;
            for (int i = 0; i < token; i++)
            {
#if NETFX_CORE
                string distance = json["rows"][0]["elements"][i]["distance"]["text"].ToString().Split(" ")[0].Replace(".", ",");
#elif __ANDROID__
                                string distance = json["rows"][0]["elements"][i]["distance"]["text"].ToString().Split(" ")[0];
#endif
                listKantorPolisi[i].distance = Convert.ToDouble(distance);
            }
            listKantorPolisi = new ObservableCollection<KantorPolisi>(listKantorPolisi.OrderBy(k => k.distance));
            gvListKantorPolisi.ItemsSource = listKantorPolisi;
        }

        public void goToDetail(object sender, ItemClickEventArgs e)
        {
            KantorPolisi selected = (KantorPolisi)e.ClickedItem;
            session.setKantorPolisi(selected);
            this.Frame.Navigate(typeof(PoliceStationDetailPage));
        }
    }
}
