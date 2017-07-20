using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Caliburn.Micro;
using Xamarin.Forms;
using ZXing.Mobile;

namespace ShareEmergencyContacts.Droid
{
    [Activity(Label = "Share emergency contacts", Theme = "@style/SplashTheme",
            MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            MobileBarcodeScanner.Initialize(Application);

            UserDialogs.Init(() => (Activity)Forms.Context);
            // stupid dependency order; can't add this interface from Application because Application executes before this
            var container = IoC.Get<SimpleContainer>();
            container.RegisterInstance(typeof(IUserDialogs), null, UserDialogs.Instance);
            container.RegisterInstance(typeof(IAppInfoProvider), null, new AndroidAppInfoProvider(Resources));

            var perm = new AndroidCheckPermissions((Activity)Forms.Context);
            OnPermissionSet += perm.PermissionRequestAnswered;

            container.RegisterInstance(typeof(ICheckPermissions), null, perm);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            LoadApplication(IoC.Get<App>());
        }

        public delegate void PermissionResponse(int requestCode, string[] permissions, Permission[] grantResults);

        public event PermissionResponse OnPermissionSet;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            OnPermissionSet?.Invoke(requestCode, permissions, grantResults);
        }
    }
}

