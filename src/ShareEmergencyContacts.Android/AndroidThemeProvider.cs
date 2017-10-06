using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidThemeProvider : IThemeProvider
    {
        private readonly MainActivity _activity;
        private NavigationPage _navigationPage;
        private bool _latestTheme;

        public AndroidThemeProvider(MainActivity activity)
        {
            _activity = activity;
        }

        public void ChangeTheme(bool darkTheme)
        {
            _latestTheme = darkTheme;
            _activity.SetTheme(darkTheme ? Resource.Style.DarkMainTheme : Resource.Style.LightMainTheme);
            if (_navigationPage != null)
            {
                _navigationPage.BarTextColor = darkTheme ? Color.White : Color.Black;
                var rc = _activity.Resources.GetColor(darkTheme ? Resource.Color.titleBarDark : Resource.Color.titleBarLight, _activity.Theme);
                _navigationPage.BackgroundColor = rc.ToColor();
            }

            var ac = _activity.Resources.GetColor(Resource.Color.accent, _activity.Theme);
            Color.SetAccent(Color.FromRgb(ac.R, ac.G, ac.B));
        }

        public void ConfigureFor(NavigationPage navigationPage)
        {
            _navigationPage = navigationPage;
            // ensure proper theme coloring when nav page gets set
            ChangeTheme(_latestTheme);
        }
    }
}