using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SahabatSurabaya.Shared.Helper;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class PersonalChatPage : Page
    {
        ObservableCollection<Chat> listChat;
        HubConnection connection;
        ChatPageParams param;
        Session session;
        User userLogin;
        HttpObject httpObject;
        double height;
        public PersonalChatPage()
        {
            this.InitializeComponent();
            listChat = new ObservableCollection<Chat>();
            lvChat.ItemsSource = listChat;
            session = new Session();
            httpObject = new HttpObject();
            height = ((Frame)Window.Current.Content).ActualHeight;
        }

        private async void pageLoaded(object sender, RoutedEventArgs e)
        {
            double h = height * 0.8-60;
            lvChat.Height = h;
            userLogin = session.getUserLogin();
            param = session.getChatPageParams();
            txtNamaUserPenerima.Text = param.nama_user_penerima;
            loadChat();
            connection = new HubConnectionBuilder()
                 .WithUrl("https://serversignalr20200907155700.azurewebsites.net/chathub")
                 .WithAutomaticReconnect()
                 .Build();

            connection.On<int,int,int,string,string,bool>("SendMessage", async (id_chat,id_user_pengirim,id_user_penerima,isi_chat,waktu_chat,isSender) =>
            {
                isSender = id_user_pengirim == userLogin.id_user ? true : false;
                listChat.Add(new Chat(id_chat,id_user_pengirim,id_user_penerima,isi_chat,waktu_chat,isSender));
                lvChat.ScrollIntoView(listChat[listChat.Count - 1]);
            });
            try
            {
                await connection.StartAsync();
            }
            catch
            {
                var messageDialog = new MessageDialog("Koneksi bermasalah");
                await messageDialog.ShowAsync();
            }
        }
        
        private async void loadChat()
        {
            string responseData = await httpObject.GetRequestWithAuthorization("user/getAllChat/" + param.id_chat,session.getTokenAuthorization());
            listChat = JsonConvert.DeserializeObject<ObservableCollection<Chat>>(responseData);
            foreach (var item in listChat)
            {
                item.isSender = item.id_user_pengirim == param.id_user_penerima ? false : true;
            }
            lvChat.ItemsSource = listChat;
            if (listChat.Count > 0)
            {
                lvChat.ScrollIntoView(listChat[listChat.Count - 1]);
            }
        }
        private async void sendChat(object sender, RoutedEventArgs e)
        {
            string chatMessage = txtChatMessage.Text;
            if (chatMessage.Length > 0)
            {
                var content = new FormUrlEncodedContent(new[]{
                    new KeyValuePair<string, string>("id_chat", param.id_chat.ToString()),
                    new KeyValuePair<string, string>("id_user_pengirim", param.id_user_pengirim.ToString()),
                    new KeyValuePair<string, string>("id_user_penerima", param.id_user_penerima.ToString()),
                    new KeyValuePair<string, string>("isi_chat", chatMessage),
                });
                string responseData = await httpObject.PostRequestUrlEncodedWithAuthorization("user/insertDetailChat", content, session.getTokenAuthorization());
                JObject json = JObject.Parse(responseData);
                if (json["status"].ToString() == "1")
                {
                    txtChatMessage.Text = "";
                    Chat chatSend = new Chat(param.id_chat, param.id_user_pengirim, param.id_user_penerima, chatMessage, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), true);
                    await connection.SendAsync("SendMessage",chatSend.id_chat,chatSend.id_user_pengirim,chatSend.id_user_penerima,chatSend.isi_chat,chatSend.waktu_chat,chatSend.isSender);
                }
            }     
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
