using Microsoft.AspNetCore.SignalR.Client;
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


namespace SahabatSurabaya
{

    public sealed partial class PersonalChatPage : Page
    {
        ObservableCollection<Chat> listChat;
        HubConnection connection;
        public PersonalChatPage()
        {
            this.InitializeComponent();
            listChat = new ObservableCollection<Chat>();
            listChat.Add(new Chat(1,1,2,"asd","23","23"));
            lvChat.ItemsSource = listChat;
        }

        private async void pageLoaded(object sender, RoutedEventArgs e)
        {
            connection = new HubConnectionBuilder()
                 .WithUrl("http://localhost:61877/ChatHub")
                 .WithAutomaticReconnect()
                 .Build();

            connection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                var m = new MessageDialog(user + "-" + message);
                await m.ShowAsync();
            });

            await connection.StartAsync();
        }
        
        private async void sendChat(object sender, RoutedEventArgs e)
        {
            string chatMessage = txtChatMessage.Text;
            if (chatMessage.Length > 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8080/");
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent("1"), "id_user_pengirim");
                    form.Add(new StringContent("2"), "id_user_penerima");
                    form.Add(new StringContent(chatMessage), "isi_chat");
                    form.Add(new StringContent(DateTime.Now.ToString("HH:mm")), "waktu_chat");
                    HttpResponseMessage response = await client.PostAsync("insertChat", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var message = new MessageDialog(response.Content.ReadAsStringAsync().Result);
                        await message.ShowAsync();
                        txtChatMessage.Text = "";
                    }
                }
            }     
        }

        private void sendMessage()
        {

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
    }
}
