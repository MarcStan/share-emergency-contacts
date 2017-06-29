using Android.App;
using Android.Content.PM;
using Android.OS;
using Caliburn.Micro;
using ZXing.Mobile;
using ZXing.Net.Mobile.Android;

namespace ShareEmergencyContacts.Droid
{
    [Activity(Label = "Share emergency contacts", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            // TODO: apparently call this only shortly before trying to scan
            MobileBarcodeScanner.Initialize(Application);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            LoadApplication(IoC.Get<App>());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            // handle camera permission
            PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

