using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace SahabatSurabaya.Shared
{
    class MapObject
    {
        public async Task openMapWithMarker(double lat, double lng, string alamat)
        {
            var location = new Location(lat, lng);
            await Map.OpenAsync(location, new MapLaunchOptions
            {
                Name = alamat,
                NavigationMode = NavigationMode.None
            });
        }

        public async Task navigateToLocation(double lat,double lng)
        {
            var location = new Location(lat, lng);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
            await Map.OpenAsync(location, options);
        }
    }
}
