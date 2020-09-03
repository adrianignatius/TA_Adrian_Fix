using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace SahabatSurabaya.Shared
{
    class MapObject
    {
        public async Task NavigateToBuilding25()
        {
            var placemark = new Placemark
            {
                CountryName = "United States",
                AdminArea = "WA",
                Thoroughfare = "Microsoft Building 25",
                Locality = "Redmond"
            };
            var options = new MapLaunchOptions { Name = "Microsoft Building 25" };

            try
            {
                await Map.OpenAsync(placemark, options);
            }
            catch (Exception ex)
            {
                // No map application available to open or placemark can not be located
            }
        }
    }
}
