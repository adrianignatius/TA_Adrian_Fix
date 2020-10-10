using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.IO;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class MakeCrimeReportPage : Page
    {
        DispatcherTimer dispatcherTimer;
        int tick = 0;
        bool isChosen = false;
        string lat, lng = "";
        Session session;
        HttpObject httpObject;
        UploadedImage imageLaporan;
        List<SettingKategori> listSetingKategoriKriminalitas;
        ObservableCollection<AutocompleteAddress> listAutoCompleteAddress;
        User userLogin;
        public MakeCrimeReportPage()
        {
            this.InitializeComponent();
            listSetingKategoriKriminalitas = new List<SettingKategori>();
            listAutoCompleteAddress = new ObservableCollection<AutocompleteAddress>();
            session = new Session();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            httpObject = new HttpObject();
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
            string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + item.description + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            string responseData = await httpObject.GetRequest(reqUri);
            JObject json = JObject.Parse(responseData);
            lat = json["results"][0]["geometry"]["location"]["lat"].ToString().Replace(",", ".");
            lng = json["results"][0]["geometry"]["location"]["lng"].ToString().Replace(",", ".");
            webViewMap.Navigate(new Uri(session.getUrlWebView() + "location-map.php?lat=" + lat + "&lng=" + lng + "&type=2"));
            listAutoCompleteAddress.Clear();
        }

        private async void searchAutocomplete()
        {
            string input = txtAutocompleteAddress.Text;
            string reqUri = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + input + "&types=geocode&location=-7.252115,112.752849&radius=20000&language=id&components=country:id&strictbounds&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            string responseData = await httpObject.GetRequest(reqUri);
            JObject json = JObject.Parse(responseData);
            if (json["status"].ToString() == "OK")
            {
                listAutoCompleteAddress.Clear();
                var token = JToken.Parse(responseData)["predictions"].ToList().Count;
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

        private bool validateInput()
        {
            if (txtJudulLaporan.Text.Length == 0 || txtAutocompleteAddress.Text.Length == 0 || cbJenisKejadian.SelectedIndex == -1 || txtDescKejadian.Text.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
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

        private void setComboBoxKategoriKriminalitas()
        {
            listSetingKategoriKriminalitas.Add(new SettingKategori("Penculikkan", "kidnap-icon.jpg"));
            listSetingKategoriKriminalitas.Add(new SettingKategori("Pencurian", "robbery-icon.png"));
            listSetingKategoriKriminalitas.Add(new SettingKategori("Perusakkan Fasilitas Umum", "vandalism-icon.png"));
            listSetingKategoriKriminalitas.Add(new SettingKategori("Tabrak Lari", "hitandrun-icon.png"));
            listSetingKategoriKriminalitas.Add(new SettingKategori("Kekerasan", "violence-icon.png"));
            listSetingKategoriKriminalitas.Add(new SettingKategori("Aktifitas Mencurigakan", "suspicious-activity.jpg"));
            cbJenisKejadian.ItemsSource = listSetingKategoriKriminalitas;
            cbJenisKejadian.DisplayMemberPath = "nama_kategori";
            cbJenisKejadian.SelectedValuePath = "nama_kategori";
        }
        public void CrimeReportPageLoaded(object sender,RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
        }

        private void deleteFile(object sender, RoutedEventArgs e)
        {
            imageLaporan = null;
            gridFile.Visibility = Visibility.Collapsed;
            txtStatusFile.Visibility = Visibility.Visible;
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
            string responseData = await httpObject.GetRequest(reqUri);
            JObject json = JObject.Parse(responseData);
            string address = json["results"][0]["formatted_address"].ToString();
            txtAutocompleteAddress.Text = address;
            webViewMap.Navigate(new Uri(session.getUrlWebView() + "location-map.php?lat=" + lat + "&lng=" + lng+"&type=2"));
        }

        public async void goToDetail(object sender, RoutedEventArgs e)
        {
            if (validateInput() == false)
            {
                var message = new MessageDialog("Ada field yang masih kosong, harap lengkapi data terlebih dahulu");
                await message.ShowAsync();
            }
            else
            {
                string responseData = await httpObject.GetRequest("checkKecamatanAvailable?lat="+lat+"&lng="+lng);
                JObject json = JObject.Parse(responseData);
                if (json["status"].ToString() == "1"){
                    string judulLaporan = txtJudulLaporan.Text;
                    string descKejadian = txtDescKejadian.Text;
                    string valueKategoriKejadian = cbJenisKejadian.SelectedValue.ToString();
                    string alamatLaporan = txtAutocompleteAddress.Text;
                    string displayJeniskejadian = listSetingKategoriKriminalitas[cbJenisKejadian.SelectedIndex].nama_kategori.ToString();
                    string tglLaporan = DateTime.Now.ToString("dd/MM/yyyy");
                    string waktuLaporan = DateTime.Now.ToString("HH:mm:ss");
                    int index = cbJenisKejadian.SelectedIndex;
                    string namaFileGambar = listSetingKategoriKriminalitas[cbJenisKejadian.SelectedIndex].file_gambar_kategori;
                    int id_kecamatan = Convert.ToInt32(json["id_kecamatan"].ToString());
                    ConfirmReportParams param = new ConfirmReportParams("kriminalitas", judulLaporan, null, descKejadian, lat, lng, alamatLaporan, id_kecamatan, tglLaporan, waktuLaporan, displayJeniskejadian, index, imageLaporan, namaFileGambar);
                    session.setConfirmreportParam(param);
                    this.Frame.Navigate(typeof(ConfirmReportPage));
                }
                else{
                    var message = new MessageDialog(json["message"].ToString());
                    await message.ShowAsync();
                }
                
            }     
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            setComboBoxKategoriKriminalitas();
            var entry = this.Frame.BackStack.LastOrDefault();
            if (entry.SourcePageType == typeof(ConfirmReportPage))
            {
                ConfirmReportParams param = session.getConfirmReportParams();
                txtJudulLaporan.Text = param.judul_laporan;
                txtDescKejadian.Text = param.deskripsi_laporan;
                txtAutocompleteAddress.Text = param.alamat_laporan;
                lat = param.lat_laporan;
                lng = param.lng_laporan;
                imageLaporan = param.image_laporan;
                cbJenisKejadian.SelectedIndex = param.combo_box_selected_index;
                if (imageLaporan != null)
                {
                    txtNamaFile.Text = imageLaporan.file_name;
                    gridFile.Visibility = Visibility.Visible;
                    txtStatusFile.Visibility = Visibility.Collapsed;
                }
                this.Frame.BackStack.RemoveAt(this.Frame.BackStack.Count - 1);
            }
        }
    }
}
