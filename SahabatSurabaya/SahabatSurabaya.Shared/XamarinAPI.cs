using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Xamarin.Essentials;

namespace SahabatSurabaya.Shared
{
    class XamarinAPI
    {
        public double lat { get; set; }
        public double lng { get; set; }

        public async Task getLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    var messageDialog = new MessageDialog(location.Latitude + " - " + location.Longitude);
                    await messageDialog.ShowAsync();
                    this.lat = location.Latitude;
                    this.lng = location.Longitude;
                }
                else
                {
                    var messageDialog = new MessageDialog("loc is null");
                    await messageDialog.ShowAsync();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                var messageDialog = new MessageDialog("Feature not supported");
                await messageDialog.ShowAsync();
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                var messageDialog = new MessageDialog("Device Exception");
                await messageDialog.ShowAsync();
            }
            catch (PermissionException pEx)
            {
                var messageDialog = new MessageDialog("Permission error");
                await messageDialog.ShowAsync();
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
                var messageDialog = new MessageDialog("Unable to get loc");
                await messageDialog.ShowAsync();
            }
        }
    }
}
