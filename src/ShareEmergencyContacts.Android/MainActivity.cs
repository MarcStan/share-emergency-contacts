﻿using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Caliburn.Micro;
using Xamarin.Forms;
using ZXing.Mobile;
using ZXing.Net.Mobile.Android;

namespace ShareEmergencyContacts.Droid
{
    [Activity(Label = "Share emergency contacts", Icon = "@drawable/icon", Theme = "@style/SplashTheme",
            MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            // TODO: apparently call this only shortly before trying to scan
            MobileBarcodeScanner.Initialize(Application);

            UserDialogs.Init(() => (Activity)Forms.Context);
            // stupid dependency order; can't add this interface from Application because Application executes before this
            var container = IoC.Get<SimpleContainer>();
            container.RegisterInstance(typeof(IUserDialogs), null, UserDialogs.Instance);
            container.RegisterInstance(typeof(IAppInfoProvider), null, new AndroidAppInfoProvider(Resources));

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            LoadApplication(IoC.Get<App>());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            // TODO: never fires, probably because we set target to android 2.3?
            // handle camera permission
            PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

