using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Essentials;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{ 
    public sealed partial class ProfilePage : Page
    {
        MapObject map;
        public ProfilePage()
        {
            this.InitializeComponent();
            map = new MapObject();
        }

        public async void goToSubscriptionPage(object sender, RoutedEventArgs e)
        {
            await map.NavigateToBuilding25();
            //this.Frame.Navigate(typeof(SubscriptionPage));
        }
    }
}
