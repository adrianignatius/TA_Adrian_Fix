using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;
using Xamarin.Essentials;

namespace SahabatSurabaya.Shared
{
    class XamarinAPI
    {
        public async void getLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    var messageDialog = new MessageDialog(location.Latitude + " - " + location.Longitude);
                    await messageDialog.ShowAsync();
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
    }
}
