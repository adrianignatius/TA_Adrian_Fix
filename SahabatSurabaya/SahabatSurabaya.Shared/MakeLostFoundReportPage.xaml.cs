using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;

namespace SahabatSurabaya
{
    public sealed partial class MakeLostFoundReportPage : Page
    {
        User userLogin;
        DispatcherTimer dispatcherTimer;
        int tick = 0;
        bool isChosen = false;
        string lat, lng = "";
        UploadedImage imageLaporan;
        ObservableCollection<AutocompleteAddress> listAutoCompleteAddress;
        List<SettingKategori> listSettingKategoriLostFound;
        Session session;
        public MakeLostFoundReportPage()
        {
            this.InitializeComponent();
            listSettingKategoriLostFound = new List<SettingKategori>();
            listAutoCompleteAddress = new ObservableCollection<AutocompleteAddress>();
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
                    lng = json["results"][0]["geometry"]["location"]["lng"].ToString().Replace(",", ".");
                    webViewMap.Navigate(new Uri(session.getUrlWebView() + "location-map.php?lat=" + lat + "&lng=" + lng));
                }
            }
            listAutoCompleteAddress.Clear();
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

        private void txtAutocompleteAddressTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Start();
            }
            tick = 0;
        }

        public async void useLocation(object sender, RoutedEventArgs e)
        {
            isChosen = true;
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                }); ;
            }
            lat = location.Latitude.ToString().Replace(",", ".");
            lng = location.Longitude.ToString().Replace(",", ".");
            using (var client = new HttpClient())
            {
                string latlng = lat + "," + lng;
                string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
                HttpResponseMessage response = await client.GetAsync(reqUri);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    JObject json = JObject.Parse(jsonString);
                    string address = json["results"][0]["formatted_address"].ToString();
                    txtAutocompleteAddress.Text = address;
                    webViewMap.Navigate(new Uri(session.getUrlWebView() + "location-map.php?lat=" + lat + "&lng=" + lng));
                }
            }
        }

        private async void LostFoundPageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            using (var client = new HttpClient()) 
            {
                client.BaseAddress = new Uri(session.getApiURL());
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getAllKategoriLostFound");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listSettingKategoriLostFound = JsonConvert.DeserializeObject<List<SettingKategori>>(responseData);
                    cbJenisBarang.ItemsSource = listSettingKategoriLostFound;
                    cbJenisBarang.DisplayMemberPath = "nama_kategori";
                    cbJenisBarang.SelectedValuePath = "id_kategori";
                }
            }
        }

        private async void goToDetail(object sender, RoutedEventArgs e)
        {
            if (validateInput() == false)
            {
                var message = new MessageDialog("Ada field yang masih kosong, harap lengkapi data terlebih dahulu");
                await message.ShowAsync();
            }
            else
            {
                int jenisLaporan;
                jenisLaporan = (bool)rbLostItem.IsChecked ? 1 : 0;
                string judulLaporan = txtJudulLaporan.Text;
                string descLaporan = txtDescBarang.Text;
                string alamatLaporan = txtAutocompleteAddress.Text;
                string displayJenisBarang = listSettingKategoriLostFound[cbJenisBarang.SelectedIndex].nama_kategori.ToString();
                string valueJenisBarang = cbJenisBarang.SelectedValue.ToString();
                string tglLaporan = DateTime.Now.ToString("dd/MM/yyyy");
                string waktuLaporan = DateTime.Now.ToString("HH:mm:ss");
                string namaFileGambar = listSettingKategoriLostFound[cbJenisBarang.SelectedIndex].file_gambar_kategori;
                ConfirmReportParams param = new ConfirmReportParams("lostfound",judulLaporan, jenisLaporan.ToString(), descLaporan, lat, lng, alamatLaporan,tglLaporan, waktuLaporan, displayJenisBarang, valueJenisBarang, imageLaporan,namaFileGambar);
                session.setConfirmreportParam(param);
                this.Frame.Navigate(typeof(ConfirmReportPage));
                //LostFoundReportParams param = new LostFoundReportParams(userLogin, judulLaporan, jenisLaporan, lat, lng, descLaporan, tglLaporan, displayJenisBarang, valueJenisBarang, imageLaporan, alamatLaporan, waktuLaporan, namaFileGambar);
                //session.setLostFoundReportDetailPageParams(param);
               // this.Frame.Navigate(typeof(LostFoundReportDetailPage));
            }

        }

        private bool validateInput()
        {
            if(txtJudulLaporan.Text.Length==0 || txtDescBarang.Text.Length==0||cbJenisBarang.SelectedIndex==-1 || txtAutocompleteAddress.Text.Length == 0)
            {
                return false;  
            }
            else
            {
                return true;
            } 
        }

        public void deleteFile(object sender, RoutedEventArgs e)
        {
            imageLaporan = null;
            txtStatusFile.Visibility = Visibility.Visible;
            gridFile.Visibility = Visibility.Collapsed;
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

        private async void chooseImage(object sender, RoutedEventArgs e)
        {
            string contents = "";
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile(new string[] { ".jpg" });
                if (fileData == null)
                {
                    return; // user canceled file picking
                }
                else
                {
                    string fileName = fileData.FileName;
                    contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
                    imageLaporan = new UploadedImage(fileName,fileData.DataArray, fileData.DataArray.Length);
                    txtNamaFile.Text = fileName;
                    gridFile.Visibility = Visibility.Visible;
                    txtStatusFile.Visibility = Visibility.Collapsed;
                }              
            }
            catch (Exception ex)
            {
                var message = new MessageDialog(ex.ToString());
                await message.ShowAsync();
            }
        }
    }
}
