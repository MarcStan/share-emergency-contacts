using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Caliburn.Micro;
using ShareEmergencyContacts.Models;
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
            // create before OnCreate
            var theme = new AndroidThemeProvider(this);
            // android needs a theme change on startup
            theme.ChangeTheme(false);

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            MobileBarcodeScanner.Initialize(Application);

            UserDialogs.Init(() => (Activity)Forms.Context);
            // stupid dependency order; can't add this interface from Application because Application executes before this
            var container = IoC.Get<SimpleContainer>();
            container.RegisterInstance(typeof(IUserDialogs), null, UserDialogs.Instance);
            container.RegisterInstance(typeof(IAppInfoProvider), null, new AndroidAppInfoProvider(Resources, "4bc7da4c-5508-436a-91fd-08ced75df7f7"));
            container.RegisterInstance(typeof(IThemeProvider), null, theme);

            var storage = new AndroidIOSStorageProvider(this);
            ActivityResultSet += storage.OnActivityResult;
            container.RegisterInstance(typeof(IStorageProvider), null, storage);

            var perm = new AndroidCheckPermissions((Activity)Forms.Context);
            OnPermissionSet += perm.PermissionRequestAnswered;

            container.RegisterInstance(typeof(ICheckPermissions), null, perm);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            LoadApplication(IoC.Get<App>());
        }

        public delegate void PermissionResponse(int requestCode, string[] permissions, Permission[] grantResults);
        public delegate void ActivityResult(int requestCode, Result resultCode, Intent data);

        public event PermissionResponse OnPermissionSet;
        public event ActivityResult ActivityResultSet;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            OnPermissionSet?.Invoke(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            ActivityResultSet?.Invoke(requestCode, resultCode, data);
        }
    }
}

