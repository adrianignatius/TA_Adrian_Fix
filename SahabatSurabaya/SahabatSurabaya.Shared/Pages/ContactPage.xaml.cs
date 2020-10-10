using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace SahabatSurabaya.Shared.Pages
{
    public sealed partial class ContactPage : Page
    {
        Session session;
        HttpObject httpObject;
        User userLogin;
        ObservableCollection<User> listEmergencyContact = new ObservableCollection<User>();
        ObservableCollection<PendingContact> listPendingContactRequest = new ObservableCollection<PendingContact>();
        ObservableCollection<User> listSentPendingContactRequest = new ObservableCollection<User>();

        public ContactPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
        }

        private void validateInput(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            if (userLogin.status_user == 0)
            {
                panelSentPendingRequest.Visibility = Visibility.Collapsed;
                panelAddContact.Visibility = Visibility.Collapsed;
                txtStatusContact.Visibility = Visibility.Visible;
            }
            else
            {
                txtStatusContact.Visibility = Visibility.Collapsed;
            }
            updateListContact();
            updateListPendingContactRequest();
            updateListSentPendingContactRequest();
        }

        private async void updateListContact()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("user/getEmergencyContact/" + userLogin.id_user,session.getTokenAuthorization());
            listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
            lvEmergencyContact.ItemsSource = listEmergencyContact;
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.GetAsync("user/getEmergencyContact/" + userLogin.id_user);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
            //        lvEmergencyContact.ItemsSource = listEmergencyContact;
            //    }
            //}
        }

        private async void updateListSentPendingContactRequest()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("user/getSentPendingContactRequest/" + userLogin.id_user,session.getTokenAuthorization());
            listSentPendingContactRequest = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
            lvSentPendingContact.ItemsSource = listSentPendingContactRequest;
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.GetAsync("user/getSentPendingContactRequest/" + userLogin.id_user);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        listSentPendingContactRequest = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
            //        lvSentPendingContact.ItemsSource = listSentPendingContactRequest;
            //    }
            //}
        }

        private async void updateListPendingContactRequest()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("user/getPendingContactRequest/" + userLogin.id_user,session.getTokenAuthorization());
            listPendingContactRequest = JsonConvert.DeserializeObject<ObservableCollection<PendingContact>>(responseData);
            lvPendingContactRequest.ItemsSource = listPendingContactRequest;
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.GetAsync("user/getPendingContactRequest/" + userLogin.id_user);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        listPendingContactRequest = JsonConvert.DeserializeObject<ObservableCollection<PendingContact>>(responseData);
            //        lvPendingContactRequest.ItemsSource = listPendingContactRequest;
            //    }
            //}
        }

        private async void addContact(object sender, RoutedEventArgs e)
        {
            if (txtSearchNumber.Text.Length != 0)
            {
                if (txtSearchNumber.Text != userLogin.telpon_user)
                {
                    var content = new FormUrlEncodedContent(new[]{
                        new KeyValuePair<string, string>("id_user_pengirim", userLogin.id_user.ToString()),
                        new KeyValuePair<string, string>("nomor_tujuan", txtSearchNumber.Text)
                    });
                    string responseData = await httpObject.PostRequestWithUrlEncoded("user/addEmergencyContact", content);
                    JObject json = JObject.Parse(responseData);
                    var message = new MessageDialog(json["message"].ToString());
                    await message.ShowAsync();
                    txtSearchNumber.Text = "";
                    updateListSentPendingContactRequest();
                    //using (var client = new HttpClient())
                    //{
                    //    client.BaseAddress = new Uri(session.getApiURL());
                    //    client.DefaultRequestHeaders.Accept.Clear();
                    //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //    MultipartFormDataContent form = new MultipartFormDataContent();
                    //    form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pengirim");
                    //    form.Add(new StringContent(txtSearchNumber.Text), "nomor_tujuan");
                    //    HttpResponseMessage response = await client.PostAsync("user/addEmergencyContact", form);
                    //    if (response.IsSuccessStatusCode)
                    //    {
                    //        var responseData = response.Content.ReadAsStringAsync().Result;
                    //        JObject json = JObject.Parse(responseData);
                    //        var message = new MessageDialog(json["message"].ToString());
                    //        await message.ShowAsync();
                    //        txtSearchNumber.Text = "";
                    //        updateListSentPendingContactRequest();
                    //    }
                    //}
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
            string id_daftar_kontak = (Sender as Button).Tag.ToString();
            string responseData = await httpObject.PutRequest("user/acceptContactRequest/" + id_daftar_kontak, null,session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            var message = new MessageDialog(json["message"].ToString());
            await message.ShowAsync();
            if (json["status"].ToString() == "1")
            {
                updateListContact();
                updateListPendingContactRequest();
            }
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpContent content = new StringContent("");
            //    HttpResponseMessage response = await client.PutAsync("user/acceptContactRequest/" +id_daftar_kontak, content);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        JObject json = JObject.Parse(responseData);
            //        var message = new MessageDialog(json["message"].ToString());
            //        await message.ShowAsync();
            //        if (json["status"].ToString() == "1")
            //        {
            //            updateListContact();
            //            updateListPendingContactRequest();
            //        } 
            //    }
            //}
        }

        private async void declineRequest(object Sender, RoutedEventArgs e)
        {
            string id_daftar_kontak = (Sender as Button).Tag.ToString();
            string responseData = await httpObject.DeleteRequest("user/declineContactRequest/" + id_daftar_kontak,session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            if (json["status"].ToString() == "1")
            {
                updateListContact();
                updateListPendingContactRequest();
            }
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    HttpResponseMessage response = await client.DeleteAsync("user/declineContactRequest/" + id_daftar_kontak);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        JObject json = JObject.Parse(responseData);
            //        if (json["status"].ToString() == "1")
            //        {
            //            updateListContact();
            //            updateListPendingContactRequest();
            //        }
            //    }
            //}
        }
    }
}
