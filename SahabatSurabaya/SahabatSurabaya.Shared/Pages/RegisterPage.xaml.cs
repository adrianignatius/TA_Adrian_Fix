using System;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using System.Timers;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using SahabatSurabaya.Shared;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya.Shared.Pages
{
    public sealed partial class RegisterPage : Page
    {
        DispatcherTimer dispatcherTimer;
        int tick = 0;
        bool isChosen = false;
        string lat = null, lng = null;
        Session session;
        HttpObject httpObject;
        ObservableCollection<AutocompleteAddress> listAutoCompleteAddress = new ObservableCollection<AutocompleteAddress>();
        public RegisterPage()
        {
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            session = new Session();
            httpObject = new HttpObject();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            tick++;
            if (tick == 2 && txtAutocompleteAddress.Text.Length!=0 && !isChosen)
            {
                searchAutocomplete();
            }
            if (isChosen && tick==2) isChosen = false;
        }

        

        private void goToLogin(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));

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

        private void txtPhoneBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void txtFullNameBeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

        private async void register(object sender, RoutedEventArgs e)
        {
            if(txtFullName.Text.Length!=0 && txtPassword.Password.Length!=0 && txtConfirmPassword.Password.Length!=0 && txtPhone.Text.Length != 0)
            {
                if (txtPassword.Password == txtConfirmPassword.Password)
                {
                    string responseData = await httpObject.GetRequest("settings/checkKecamatanAvailable?lat=" + lat + "&lng=" + lng);
                    JObject json = JObject.Parse(responseData);
                    if (json["status"].ToString() == "1")
                    {
                        var formContent = new Dictionary<string, string>();
                        formContent.Add("nama_user", txtFullName.Text);
                        formContent.Add("telpon_user", txtPhone.Text);
                        formContent.Add("password_user", txtPassword.Password);
                        if (txtAutocompleteAddress.Text.Length != 0)
                        {
                            formContent.Add("alamat_available", "1");
                            formContent.Add("lokasi_aktif_user", txtAutocompleteAddress.Text);
                            formContent.Add("lat_user", lat);
                            formContent.Add("lng_user", lng);
                        }
                        else
                        {
                            formContent.Add("alamat_available", "0");
                        }
                        responseData = await httpObject.PostRequestWithUrlEncoded("registerUser", new FormUrlEncodedContent(formContent));
                        json = JObject.Parse(responseData);
                        var message = new MessageDialog(json["message"].ToString());
                        await message.ShowAsync();
                        if (json["status"].ToString() == "1")
                        {
                            string data = json["data"].ToString();
                            User userRegister = JsonConvert.DeserializeObject<User>(data);
                            session.setUserLogin(userRegister);
                            session.setTokenAuthorization(json["token"].ToString());
                            this.Frame.Navigate(typeof(VerifyOtpPage));
                        }
                    }
                    else
                    {
                        var message = new MessageDialog(json["message"].ToString());
                        await message.ShowAsync();
                    }
                    
                }
                else
                {
                    var message = new MessageDialog("Confirm password tidak sesuai dengan password yang dimasukkan");
                    await message.ShowAsync();
                }
            }
            else
            {
                var message = new MessageDialog("Ada field yang masih kosong, harap lengkapi data terlebih dahulu");
                await message.ShowAsync();
            }
            
            
        }
    }
}
