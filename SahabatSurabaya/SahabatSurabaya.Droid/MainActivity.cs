using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SahabatSurabaya.Droid
{
    [Activity(
            MainLauncher = true,
            ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
            WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
        )]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            OneSignal.Current.StartInit("6fd226ba-1d41-4c7b-9f8b-a973a8fd436b")
                             .Settings(new Dictionary<string, bool>() {
                                            { IOSSettings.kOSSettingsKeyAutoPrompt, false },
                                            { IOSSettings.kOSSettingsKeyInAppLaunchURL, false } })
                             .InFocusDisplaying(OSInFocusDisplayOption.Notification)
                             .EndInit();
            OneSignal.Current.RegisterForPushNotifications();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
    }
    
}

