using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactPage : Page
    {
        Session session;
        User userLogin;
        ObservableCollection<User> listEmergencyContact = new ObservableCollection<User>();
        ObservableCollection<User> listPendingContactRequest = new ObservableCollection<User>();
        ObservableCollection<User> listSentPendingContactRequest = new ObservableCollection<User>();

        public ContactPage()
        {
            this.InitializeComponent();
            session = new Session();

        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            updateListContact();
            updateListPendingContactRequest();
            updateListSentPendingContactRequest();
        }

        private async void updateListContact()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("user/getEmergencyContact/" + userLogin.id_user);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
                    lvEmergencyContact.ItemsSource = listEmergencyContact;
                }
            }
        }

        private async void updateListSentPendingContactRequest()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("user/getSentPendingContactRequest/" + userLogin.id_user);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listSentPendingContactRequest = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
                    lvSentPendingContact.ItemsSource = listSentPendingContactRequest;
                }
            }
        }

        private async void updateListPendingContactRequest()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("user/getPendingContactRequest/" + userLogin.id_user);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listPendingContactRequest = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
                    lvPendingContactRequest.ItemsSource = listPendingContactRequest;
                }
            }
        }

        private async void addContact(object sender, RoutedEventArgs e)
        {
            if (txtSearchNumber.Text.Length != 0)
            {
                if (txtSearchNumber.Text != userLogin.telpon_user)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:8080/");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        MultipartFormDataContent form = new MultipartFormDataContent();
                        form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pengirim");
                        form.Add(new StringContent(txtSearchNumber.Text), "nomor_tujuan");
                        HttpResponseMessage response = await client.PostAsync("user/addEmergencyContact", form);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = response.Content.ReadAsStringAsync().Result;
                            JObject json = JObject.Parse(responseData);
                            var message = new MessageDialog(json["message"].ToString());
                            await message.ShowAsync();
                            txtSearchNumber.Text = "";
                        }
                    }
                }
                else
                {
                    var message = new MessageDialog("Tidak bisa menambahkan nomor sendiri ke dalam daftar kontak");
                    await message.ShowAsync();
                }
            }
            else
            {
                var message = new MessageDialog("Inputkan terlebih dahulu nomor tujuan yang ingin ditambahkan.");
                await message.ShowAsync();
            }
        }

        private async void acceptRequest(object Sender, RoutedEventArgs e)
        {
            var m = new MessageDialog((Sender as Button).Tag.ToString());
            await m.ShowAsync();
        }

        private async void declineRequest(object Sender, RoutedEventArgs e)
        {
            var m = new MessageDialog((Sender as Button).Tag.ToString());
            await m.ShowAsync();
        }
    }
}
