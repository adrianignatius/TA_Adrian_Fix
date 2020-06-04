using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MakeLostFoundReportPage : Page
    {
        int imageCount = 0;
        List<UploadedImage> listImage;
        List<SettingKategori> listSettingKategoriLostFound;
        public MakeLostFoundReportPage()
        {
            this.InitializeComponent();
            listImage = new List<UploadedImage>();
            listSettingKategoriLostFound = new List<SettingKategori>();
        }

        private async void LostFoundPageLoaded(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getAllKategoriLostFound");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var responseData = response.Content.ReadAsStringAsync().Result;

                    //int length = ((JArray)json["data"]).Count;
                    listSettingKategoriLostFound = JsonConvert.DeserializeObject<List<SettingKategori>>(responseData);
                    cbJenisBarang.ItemsSource = listSettingKategoriLostFound;
                    cbJenisBarang.DisplayMemberPath = "nama_kategori";
                    cbJenisBarang.SelectedValuePath = "id_kategori";
                }
            }
        }

        private async void goToDetail(object sender, RoutedEventArgs e)
        {
            int jenisLaporan;
            jenisLaporan = (bool)rbLostItem.IsChecked ? 1 : 0;
            string judulLaporan = txtJudulLaporan.Text;
            string descLaporan = txtDescBarang.Text;
            string[] getAddress = new string[] { @"document.getElementById('valueAddress').value" };
            string alamatLaporan = await webViewMap.InvokeScriptAsync("eval", getAddress);
            string displayJenisBarang = listSettingKategoriLostFound[cbJenisBarang.SelectedIndex].nama_kategori.ToString();
            string valueJenisBarang = cbJenisBarang.SelectedValue.ToString();
            string[] getLat = new string[] { @"document.getElementById('valueLat').value" };
            string lat = await webViewMap.InvokeScriptAsync("eval", getLat);
            var s = new MessageDialog(lat.ToString());
            await s.ShowAsync();
            string[] getLng = new string[] { @"document.getElementById('valueLng').value" };
            string lng = await webViewMap.InvokeScriptAsync("eval", getLng);
            string tglLaporan = DateTime.Now.ToString("dd/MM/yyyy");
            string waktuLaporan = DateTime.Now.ToString("HH:mm:ss");
            string namaFileGambar = listSettingKategoriLostFound[cbJenisBarang.SelectedIndex].file_gambar_kategori;
            LostFoundReportParams param = new LostFoundReportParams(judulLaporan, jenisLaporan, lat, lng, descLaporan, tglLaporan, displayJenisBarang, valueJenisBarang, listImage, alamatLaporan, waktuLaporan, namaFileGambar);
            this.Frame.Navigate(typeof(LostFoundReportDetailPage), param);
        }
        public void updateTxtImageCount()
        {
            txtImageCount.Text = imageCount + " gambar terpilih(Max. 2 Gambar)";
        }
        async void deleteFile(object sender, RoutedEventArgs e)
        {
            Button selectedBtn = sender as Button;
            listImage.RemoveAt(Convert.ToInt32(selectedBtn.Tag));
            stackFile.Children.Remove((UIElement)this.FindName("sp" + selectedBtn.Tag.ToString()));
            imageCount--;
            updateTxtImageCount();
        }

        private async void chooseImage(object sender, RoutedEventArgs e)
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
                    contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
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
