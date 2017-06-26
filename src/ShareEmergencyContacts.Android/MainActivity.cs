using Android.App;
using Android.Content.PM;
using Android.OS;
using Caliburn.Micro;

namespace ShareEmergencyContacts.Droid
{
    [Activity(Label = "Share emergency contacts", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(IoC.Get<App>());
        }
    }
}

