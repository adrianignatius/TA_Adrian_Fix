using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MakeCrimeReportPage : Page
    {
        int imageCount = 0;
        List<UploadedImage> listImage;
        List<SettingKategori> listSetingKategoriKriminalitas;
        public MakeCrimeReportPage()
        {
            this.InitializeComponent();
            listImage = new List<UploadedImage>();
            listSetingKategoriKriminalitas = new List<SettingKategori>();
        }
        public async void CrimeReportPageLoaded(object sender,RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getAllKategoriCrime");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listSetingKategoriKriminalitas = JsonConvert.DeserializeObject<List<SettingKategori>>(responseData);
                    cbJenisKejadian.ItemsSource = listSetingKategoriKriminalitas;
                    cbJenisKejadian.DisplayMemberPath = "nama_kategori";
                    cbJenisKejadian.SelectedValuePath = "id_kategori";
                }
            }
        }
        async void deleteFile(object sender, RoutedEventArgs e)
        {
            Button selectedBtn = sender as Button;
            listImage.RemoveAt(Convert.ToInt32(selectedBtn.Tag));
            stackFile.Children.Remove((UIElement)this.FindName("sp" + selectedBtn.Tag.ToString()));
            imageCount--;
            updateTxtImageCount();
        }

        public void updateTxtImageCount()
        {
            txtImageCount.Text = imageCount + " gambar terpilih(Max. 2 Gambar)";
        }

        public async void useLocation(object sender, RoutedEventArgs e)
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                }); ;
            }
            var messageBox = new MessageDialog(location.Latitude + "-" + location.Longitude);
            await messageBox.ShowAsync();
            string[] args = {location.Latitude.ToString(), location.Longitude.ToString() };
            string lat = await webViewMap.InvokeScriptAsync("myFunction", args);
        }

        public async void goToDetail(object sender, RoutedEventArgs e)
        {
            string judulLaporan = txtJudulLaporan.Text;
            string descKejadian = txtDescKejadian.Text;
            string valueKategoriKejadian = cbJenisKejadian.SelectedValue.ToString();
            string[] getAddress = new string[] { @"document.getElementById('valueAddress').value" };
            string alamatLaporan = await webViewMap.InvokeScriptAsync("eval", getAddress);
            string[] getLat = new string[] { @"document.getElementById('valueLat').value" };
            string lat = await webViewMap.InvokeScriptAsync("eval", getLat);
           
        }

        public async void chooseImage(object sender, RoutedEventArgs e)
        {
            if (imageCount < 3)
            {
                string contents = "";
                try
                {
                    FileData fileData = await CrossFilePicker.Current.PickFile(new string[] { ".jpg" });
                    if (fileData == null)
                    {
                        return; // user canceled file picking
                    }
                    string fileName = fileData.FileName;
                    UploadedImage imageBaru = new UploadedImage(fileData.DataArray, fileData.DataArray.Length);
                    listImage.Add(imageBaru);
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    sp.Name = "sp" + imageCount.ToString();
                    sp.Margin = new Thickness(0, 5, 0, 0);
                    TextBlock newFile = new TextBlock();
                    newFile.Text = fileData.FileName;
                    newFile.FontSize = 15;
                    newFile.Width = 250;
                    newFile.Margin = new Thickness(25, 0, 0, 0);
                    newFile.TextWrapping = TextWrapping.Wrap;
                    sp.Children.Add(newFile);
                    Button btnClose = new Button();
                    btnClose.Content = "X";
                    btnClose.Click += deleteFile;
                    btnClose.Tag = imageCount.ToString();
                    btnClose.Name = "btn" + imageCount.ToString();
                    sp.Children.Add(btnClose);
                    stackFile.Children.Add(sp);
                    imageCount++;
                    updateTxtImageCount();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception choosing file: " + ex.ToString());
                }
            }
            else
            {
                var message = new MessageDialog("Anda hanya dapat mengupload maksimal 3 gambar saja");
                message.ShowAsync();
            }
        }
    }
}
