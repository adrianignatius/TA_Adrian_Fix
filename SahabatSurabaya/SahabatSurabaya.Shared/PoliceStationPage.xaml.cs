using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{

    public sealed partial class PoliceStationPage : Page
    {
        public PoliceStationPage()
        {
            this.InitializeComponent();
        }
        ObservableCollection<KantorPolisi> listKantorPolisi;
        Session session;
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
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getAllKantorPolisi");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    string destinations = "";
                    listKantorPolisi = JsonConvert.DeserializeObject<ObservableCollection<KantorPolisi>>(responseData);
                    for (int i = 0; i < listKantorPolisi.Count; i++)
                    {
                       destinations+= listKantorPolisi[i].lat_kantor_polisi.Replace(",", ".") + "," + listKantorPolisi[i].lng_kantor_polisi.Replace(",", ".")+"|";
                    }
                    destinations.Remove(destinations.Length - 1, 1);
                    using (var client2 = new HttpClient())
                    {
                        string reqUri = "https://maps.googleapis.com/maps/api/distancematrix/json?units=metrics&origins=" + origins + "&destinations=" + destinations + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
                        HttpResponseMessage response2 = await client2.GetAsync(reqUri);
                        if (response2.IsSuccessStatusCode)
                        {
                            var hasil = response2.Content.ReadAsStringAsync().Result;
                            JObject json = JObject.Parse(hasil);
                            var token = JToken.Parse(hasil)["rows"][0]["elements"].ToList().Count;
                            for (int i = 0; i < token; i++)
                            {
#if NETFX_CORE
                                string distance = json["rows"][0]["elements"][i]["distance"]["text"].ToString().Split(" ")[0].Replace(".",",");
#elif __ANDROID__
                                string distance = json["rows"][0]["elements"][i]["distance"]["text"].ToString().Split(" ")[0];
#endif
                                listKantorPolisi[i].distance = Convert.ToDouble(distance);
                            }
                        }
                        else
                        {
                            var message = new MessageDialog(response2.StatusCode.ToString());
                            await message.ShowAsync();
                        }
                    }
                    listKantorPolisi=new ObservableCollection<KantorPolisi>(listKantorPolisi.OrderBy(k=>k.distance));
                    gvListKantorPolisi.ItemsSource = listKantorPolisi;
                }
            }
        }

        public void goToDetail(object sender, ItemClickEventArgs e)
        {
            KantorPolisi selected = (KantorPolisi)e.ClickedItem;
            session.setKantorPolisi(selected);
            this.Frame.Navigate(typeof(PoliceStationDetailPage));
        }
    }
}
