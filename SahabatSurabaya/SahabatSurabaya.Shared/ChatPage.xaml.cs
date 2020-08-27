using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SahabatSurabaya.Shared;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        User userLogin;
        ObservableCollection<DisplayHeaderChat> listDisplayHeaderChat;
        public ChatPage()
        {
            this.InitializeComponent();
        }
        
        private async void loadHeaderChat()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept.Clear();
                HttpResponseMessage response = await client.GetAsync("getHeaderChat/"+userLogin.id_user);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    ObservableCollection<HeaderChat>tempHeaderChat = JsonConvert.DeserializeObject<ObservableCollection<HeaderChat>>(responseData);
                    for (int i = 0; i < tempHeaderChat.Count; i++)
                    {
                        HttpResponseMessage response2 = await client.GetAsync("getLastMessage/" + tempHeaderChat[i].id_chat);
                        if (response2.IsSuccessStatusCode)
                        {
                            var jsonString = await response2.Content.ReadAsStringAsync();
                            //listDisplayHeaderChat.Add(new DisplayHeaderChat());
                        }
                    }
                    
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            userLogin = e.Parameter as User;
            loadHeaderChat();
        }

    }
}
