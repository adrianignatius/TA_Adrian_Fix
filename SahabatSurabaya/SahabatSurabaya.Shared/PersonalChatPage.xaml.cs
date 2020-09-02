using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            lvChat.ItemsSource = listChat;
        }

        private async void pageLoaded(object sender, RoutedEventArgs e)
        {
            connection = new HubConnectionBuilder()
                 .WithUrl("http://localhost:61877/ChatHub")
                 .WithAutomaticReconnect()
                 .Build();

            connection.On<int,int,int,string,string,bool>("SendMessage", async (id_chat,id_user_pengirim,id_user_penerima,isi_chat,waktu_chat,isSender) =>
            {
                listChat.Add(new Chat(id_chat,id_user_pengirim,id_user_penerima,isi_chat,waktu_chat,isSender));
                svChat.ChangeView(0.0f, double.MaxValue, 1.0f);
            });

            await connection.StartAsync();
            loadChat();
            svChat.ChangeView(0.0f, double.MaxValue, 1.0f);
        }
        
        private async void loadChat()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("/getAllChat/"+param.id_chat);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    listChat = JsonConvert.DeserializeObject<ObservableCollection<Chat>>(responseData);
                    foreach(var item in listChat)
                    {
                        if (item.id_user_pengirim == param.id_user_penerima)
                        {
                            item.isSender = false;
                        }
                        else
                        {
                            item.isSender = true;
                        }
                    }
                    lvChat.ItemsSource = listChat;
                }
                svChat.ChangeView(0.0f, double.MaxValue, 1.0f);
                svChat.ChangeView(0.0f, double.MaxValue, 1.0f);
            }
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
                    form.Add(new StringContent(param.id_chat.ToString()), "id_chat");
                    form.Add(new StringContent(param.id_user_pengirim.ToString()), "id_user_pengirim");
                    form.Add(new StringContent(param.id_user_penerima.ToString()), "id_user_penerima");
                    form.Add(new StringContent(chatMessage), "isi_chat");
                    form.Add(new StringContent(DateTime.Now.ToString("HH:mm")), "waktu_chat");
                    HttpResponseMessage response = await client.PostAsync("insertDetailChat", form);
                    if (response.IsSuccessStatusCode)
                    {
                        txtChatMessage.Text = "";
                        Chat chatSend = new Chat(param.id_chat, param.id_user_pengirim, param.id_user_penerima, chatMessage, DateTime.Now.ToString("HH:mm"),true);
                        await connection.SendAsync("SendMessage",chatSend.id_chat,chatSend.id_user_pengirim,chatSend.id_user_penerima,chatSend.isi_chat,chatSend.waktu_chat,chatSend.isSender);
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
