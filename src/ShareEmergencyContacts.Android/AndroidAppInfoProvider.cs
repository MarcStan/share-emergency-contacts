using Android.Content.Res;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidAppInfoProvider : IAppInfoProvider
    {
        private readonly Resources _resources;

        public AndroidAppInfoProvider(Resources resources)
        {
            _resources = resources;
        }

        public int ScreenWidth => _resources.DisplayMetrics.WidthPixels;

        public int ScreenHeight => _resources.DisplayMetrics.HeightPixels;
    }

}