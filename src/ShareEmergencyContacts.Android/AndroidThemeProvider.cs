using Xamarin.Forms;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidThemeProvider : IThemeProvider
    {
        private readonly MainActivity _activity;

        public AndroidThemeProvider(MainActivity activity)
        {
            _activity = activity;
        }

        public void ChangeTheme(bool darkTheme)
        {
            if (Application.Current?.Resources?.MergedWith == null)
                return;

            _activity.SetTheme(darkTheme ? Resource.Style.DarkMainTheme : Resource.Style.LightMainTheme);
        }
    }
}