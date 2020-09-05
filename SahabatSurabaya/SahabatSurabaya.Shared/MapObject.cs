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
            try
            {
                await Map.OpenAsync(location, new MapLaunchOptions
                {
                    Name = alamat,
                    NavigationMode=NavigationMode.None
                }) ;
            }
            catch (Exception ex)
            {
                // No map application available to open or placemark can not be located
            }
        }
    }
}
