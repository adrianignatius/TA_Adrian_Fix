using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        HttpObject httpObject;
        ObservableCollection<AutocompleteAddress> listAutoCompleteAddress;
        List<SettingKategori> listSettingKategoriLostFound;
        Session session;
        public MakeLostFoundReportPage()
        {
            this.InitializeComponent();
            listSettingKategoriLostFound = new List<SettingKategori>();
            listAutoCompleteAddress = new ObservableCollection<AutocompleteAddress>();
            session = new Session();
            httpObject = new HttpObject();
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
                        lvSuggestion.IsItemClickEnabled = true;
                    }
                    else
                    {
                        if (txtAutocompleteAddress.Text.Length != 0)
                        {
                            listAutoCompleteAddress.Clear();
                            listAutoCompleteAddress.Add(new AutocompleteAddress("Tidak ada hasil ditemukan", ""));
                            lvSuggestion.ItemsSource = listAutoCompleteAddress;
                            lvSuggestion.IsItemClickEnabled = false;
                        }
                    }
                }
            }
        }

        private void setComboBoxKategoriLostFound()
        {
            listSettingKategoriLostFound.Add(new SettingKategori("Handphone", "handphone-icon.png"));
            listSettingKategoriLostFound.Add(new SettingKategori("Tas", "bag-icon.png"));
            listSettingKategoriLostFound.Add(new SettingKategori("Jam Tangan", "watch-icon.png"));
            listSettingKategoriLostFound.Add(new SettingKategori("Perhiasan", "jewerly-icon.png"));
            listSettingKategoriLostFound.Add(new SettingKategori("Sepatu", "shoes-icon.png"));
            listSettingKategoriLostFound.Add(new SettingKategori("Hewan Peliharaan", "pet-icon.png"));
            listSettingKategoriLostFound.Add(new SettingKategori("Dompet", "wallet-icon.png"));
            cbJenisBarang.ItemsSource = listSettingKategoriLostFound;
            cbJenisBarang.DisplayMemberPath = "nama_kategori";
            cbJenisBarang.SelectedValuePath = "nama_kategori";
        }

        private void txtAutocompleteAddressTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAutocompleteAddress.Text.Length == 0)
            {
                listAutoCompleteAddress.Clear();
            }
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
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(30)
                }); ;
            }
            lat = location.Latitude.ToString().Replace(",", ".");
            lng = location.Longitude.ToString().Replace(",", ".");
            string latlng = lat + "," + lng;
            string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            string responseData = await httpObject.GetRequest(reqUri,session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            string address = json["results"][0]["formatted_address"].ToString();
            txtAutocompleteAddress.Text = address;
            string type = (bool)rbLostItem.IsChecked ? "1" : "0";
            webViewMap.Navigate(new Uri(session.getUrlWebView() + "location-map.php?lat=" + lat + "&lng=" + lng+"&type="+type));
        }

        private void LostFoundPageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
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
                if (imageLaporan != null)
                {   
                    int jenisLaporan = (bool)rbLostItem.IsChecked ? 1 : 0;
                    string judulLaporan = txtJudulLaporan.Text;
                    string descLaporan = txtDescBarang.Text;
                    string alamatLaporan = txtAutocompleteAddress.Text;
                    string displayJenisBarang = listSettingKategoriLostFound[cbJenisBarang.SelectedIndex].nama_kategori.ToString();
                    string tglLaporan = DateTime.Now.ToString("dd/MM/yyyy");
                    string waktuLaporan = DateTime.Now.ToString("HH:mm:ss");
                    int index = cbJenisBarang.SelectedIndex;
                    string namaFileGambar = listSettingKategoriLostFound[cbJenisBarang.SelectedIndex].file_gambar_kategori;
                    ConfirmReportParams param = new ConfirmReportParams("lostfound", judulLaporan, jenisLaporan.ToString(), descLaporan, lat, lng, alamatLaporan, tglLaporan, waktuLaporan, displayJenisBarang, index, imageLaporan, namaFileGambar);
                    session.setConfirmreportParam(param);
                    this.Frame.Navigate(typeof(ConfirmReportPage));
                }
                else
                {
                    var message = new MessageDialog("Wajib menyertakan gambar untuk membuat laporan Lost & Found");
                    await message.ShowAsync();
                }
                
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            setComboBoxKategoriLostFound();
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(ConfirmReportPage))
            {
                ConfirmReportParams param = session.getConfirmReportParams();
                txtJudulLaporan.Text = param.judul_laporan;
                txtDescBarang.Text = param.deskripsi_laporan;
                txtAutocompleteAddress.Text = param.alamat_laporan;
                lat = param.lat_laporan;
                lng = param.lng_laporan;
                imageLaporan = param.image_laporan;
                cbJenisBarang.SelectedIndex = param.combo_box_selected_index;
                if (imageLaporan != null)
                {
                    txtNamaFile.Text = imageLaporan.file_name;
                    gridFile.Visibility = Visibility.Visible;
                    txtStatusFile.Visibility = Visibility.Collapsed;
                }
                this.Frame.BackStack.RemoveAt(this.Frame.BackStack.Count - 1);
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
            this.Frame.GoBack();
        }

        private async void chooseImage(object sender, RoutedEventArgs e)
        {
            var customFileType =
            new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new string[] { "image/png", "image/jpeg"} },
                { DevicePlatform.UWP, new string[] { ".jpg", ".png" } }
            });
            var options = new PickOptions
            {
                PickerTitle = "Pilih Gambar..",
                FileTypes = customFileType,
            };
            var result = await FilePicker.PickAsync(options);
            byte[] hasil;
            if (result != null)
            {
                Stream stream = await result.OpenReadAsync();
                using (var streamReader = new MemoryStream())
                {
                    stream.CopyTo(streamReader);
                    hasil = streamReader.ToArray();
                }
                string fileName = result.FileName;
                imageLaporan = new UploadedImage(fileName, hasil, 0);
                txtNamaFile.Text = fileName;
                gridFile.Visibility = Visibility.Visible;
                txtStatusFile.Visibility = Visibility.Collapsed;
            }
        }
    }
}
