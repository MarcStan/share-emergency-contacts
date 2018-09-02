using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Themes;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidThemeProvider : IThemeProvider
    {
        private readonly MainActivity _activity;
        private NavigationPage _navigationPage;

        public AndroidThemeProvider(MainActivity activity)
        {
            _activity = activity;
        }

        public bool IsDarkTheme { get; private set; }

        public void ChangeTheme(bool darkTheme)
        {
            if (App.Current != null)
            {
                // not sure how to fix the warning yet https://github.com/xamarin/Xamarin.Forms/pull/1229
                // need the uri for the external assembly
                App.Current.Resources.MergedWith = darkTheme ? typeof(DarkThemeResources) : typeof(LightThemeResources);
            }
            IsDarkTheme = darkTheme;
            _activity.SetTheme(darkTheme ? Resource.Style.DarkMainTheme : Resource.Style.LightMainTheme);
            if (_navigationPage != null)
            {
                var rc = _activity.Resources.GetColor(darkTheme ? Resource.Color.titleBarDark : Resource.Color.titleBarLight, _activity.Theme);
                _navigationPage.BackgroundColor = rc.ToColor();

                _navigationPage.BarTextColor = darkTheme ? Color.White : Color.Black;
            }

            var ac = _activity.Resources.GetColor(Resource.Color.accent, _activity.Theme);
            Color.SetAccent(Color.FromRgb(ac.R, ac.G, ac.B));
        }

        public void ConfigureFor(NavigationPage navigationPage)
        {
            _navigationPage = navigationPage;
            // ensure proper theme coloring when nav page gets set
            ChangeTheme(IsDarkTheme);
        }
    }
}