using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // force disable fullscreen
            Window.ClearFlags(WindowManagerFlags.Fullscreen);
            // create before OnCreate
            var theme = new AndroidThemeProvider(this);
            // android needs a theme change on startup
            theme.ChangeTheme(false);

            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);

            MobileBarcodeScanner.Initialize(Application);

            UserDialogs.Init(() => this);

            // Use a static instance because this application class is only ever instantiated once: on the first app launch.
            // If a user closes the app (pushed to background) and clicks the app icon again, a new app is launched.
            // BUT: this application is not created again, only the MainActivity.OnCreate/OnResume functions are called

            // this method executed again when app is in background but user presses on app icon
            // it seems LoadApplication(IoC.Get<App>()); doesn't load the app then, because it's already loaded -> we get a white screen forever
            // if we however create a new container, all is well
            // I 'm pretty sure this is a serious memory leak and we probably end up with everything loaded twice (or more times)
            // let's hope caliburn micro 4 introduces a better loading model
            var container = CaliburnApplication.Container = new SimpleContainer();
            container.Instance(CaliburnApplication.Container);
            container.Singleton<App>();
            container.RegisterInstance(typeof(IPhoneDialProvider), null, new AndroidPhoneDialProvider(BaseContext));
            container.RegisterInstance(typeof(IClipboardProvider), null, new AndroidClipboardProvider(BaseContext));
            container.RegisterInstance(typeof(IShareProvider), null, new AndroidShareProvider());
            container.RegisterInstance(typeof(IUnhandledExceptionHandler), null, new AndroidUnhandledExceptionHandler());
            container.RegisterInstance(typeof(IUserDialogs), null, UserDialogs.Instance);
            container.RegisterInstance(typeof(IAppInfoProvider), null, new AndroidAppInfoProvider(Resources, "4bc7da4c-5508-436a-91fd-08ced75df7f7"));
            container.RegisterInstance(typeof(IThemeProvider), null, theme);

            var storage = new AndroidIOSStorageProvider(this);
            ActivityResultSet += storage.OnActivityResult;
            container.RegisterInstance(typeof(IStorageProvider), null, storage);

            var perm = new AndroidCheckPermissions(this);
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

