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
        ChatPageParams param;
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

            connection.On("SendMessage", async () =>
            {
                var m = new MessageDialog("Test");
                await m.ShowAsync();
            });

            //connection.On<string, string>("SendMessage", async (user, message) =>
            //{
            //    var m = new MessageDialog(user + "-" + message);
            //    await m.ShowAsync();
            //});

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
                    form.Add(new StringContent(param.id_user_pengirim.ToString()), "id_user_pengirim");
                    form.Add(new StringContent(param.id_user_penerima.ToString()), "id_user_penerima");
                    form.Add(new StringContent(chatMessage), "isi_chat");
                    form.Add(new StringContent(DateTime.Now.ToString("HH:mm")), "waktu_chat");
                    HttpResponseMessage response = await client.PostAsync("insertDetailChat", form);
                    if (response.IsSuccessStatusCode)
                    {
                        txtChatMessage.Text = "";
                        await connection.SendAsync("SendMessage");
                    }
                }
            }     
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();     
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as ChatPageParams;
            txtNamaUserPenerima.Text = param.nama_user_penerima;
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
