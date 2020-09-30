using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace SahabatSurabaya
{ 
    public sealed partial class ProfilePage : Page
    {
        Session session;
        User userLogin;
        DispatcherTimer dispatcherTimer;
        string lat = "default", lng = "default";
        ObservableCollection<AutocompleteAddress> listAutoCompleteAddress = new ObservableCollection<AutocompleteAddress>();
        int tick = 0;
        bool isChosen = false;
        public ProfilePage()
        {
            this.InitializeComponent();
            session = new Session();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            tick++;
            if (tick == 2 && txtAutocompleteAddress.Text.Length != 0 && !isChosen)
            {
                searchAutocomplete();
            }
            if (isChosen && tick == 2) isChosen = false;
        }

        private async void searchAutocomplete()
        {
            string input = txtAutocompleteAddress.Text;
            using (var client = new HttpClient())
            {
                string reqUri = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + input + "&types=geocode&location=-7.252115,112.752849&radius=20000&language=id&components=country:id&strictbounds&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
                HttpResponseMessage response = await client.GetAsync(reqUri);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(jsonString);
                    if (json["status"].ToString() == "OK")
                    {
                        listAutoCompleteAddress.Clear();
                        var token = JToken.Parse(jsonString)["predictions"].ToList().Count;
                        for (int i = 0; i < token; i++)
                        {
                            string description = json["predictions"][i]["description"].ToString();
                            string placeId = json["predictions"][i]["place_id"].ToString();
                            listAutoCompleteAddress.Add(new AutocompleteAddress(description, placeId));
                        }
                        lvSuggestion.ItemsSource = listAutoCompleteAddress;
                    }
                    else
                    {
                        if (txtAutocompleteAddress.Text.Length != 0)
                        {
                            listAutoCompleteAddress.Clear();
                            listAutoCompleteAddress.Add(new AutocompleteAddress("Tidak ada hasil ditemukan", ""));
                            lvSuggestion.ItemsSource = listAutoCompleteAddress;
                        }
                    }
                }
            }
        }


        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            txtNotelpUser.Text = userLogin.telpon_user;
            txtNamaUser.Text = userLogin.nama_user;
            txtEmailUser.Text = userLogin.email_user;
            if (userLogin.lokasi_aktif_user == null)
            {
                txtStatusLokasiAktif.Visibility = Visibility.Visible;     
            }
            else
            {
                txtStatusLokasiAktif.Visibility = Visibility.Collapsed;
                txtAutocompleteAddress.Text = userLogin.lokasi_aktif_user;
            }
            if (userLogin.status_user == 0)
            {
                btnSubscribe.Visibility = Visibility.Collapsed;
            }
        }

        private void txtAutocompleteAddressTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
            }
            tick = 0;
        }

        private async void suggestionChosen(object sender, ItemClickEventArgs e)
        {
            isChosen = true;
            AutocompleteAddress item = (AutocompleteAddress)e.ClickedItem;
            txtAutocompleteAddress.Text = item.description;
            using (var client = new HttpClient())
            {
                string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + item.description + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
                HttpResponseMessage response = await client.GetAsync(reqUri);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(jsonString);
                    lat = json["results"][0]["geometry"]["location"]["lat"].ToString().Replace(",", ".");
                    lng = json["results"][0]["geometry"]["location"]["lng"].ToString().Replace(",", "."); ;
                }
            }
            listAutoCompleteAddress.Clear();
        }

        private void goToSubscriptionPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SubscriptionPage));
        }
        
        private void changePassword(object sender,RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChangePasswordPage));
        }
    }
}
