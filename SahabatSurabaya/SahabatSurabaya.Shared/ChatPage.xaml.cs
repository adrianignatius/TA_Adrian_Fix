using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        HubConnection connection;
        public ChatPage()
        {
            this.InitializeComponent();
        }

        public async void pageLoaded(object sender, RoutedEventArgs e)
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
        
        public async void coba(object sender, RoutedEventArgs e)
        {
            await connection.InvokeAsync("SendMessage", "asd", "abc");
        }

    }
}
