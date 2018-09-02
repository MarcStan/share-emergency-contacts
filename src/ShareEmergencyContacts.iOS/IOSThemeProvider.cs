using Xamarin.Forms;
using Xamarin.Forms.Themes;

namespace ShareEmergencyContacts.iOS
{
    public class IOSThemeProvider : IThemeProvider
    {
        private NavigationPage _navigationPage;

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
            if (_navigationPage != null)
            {
                _navigationPage.BarBackgroundColor = darkTheme ? Color.Black : Color.White;
                _navigationPage.BarTextColor = darkTheme ? Color.White : Color.Black;

                // despite what the tutorials say this DOES NOT WORK
                // the next line makes it NOT WORK anymore, removing it fixes it (BarTextColor sets it correctly already)
                // UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.BlackOpaque, false);
            }
        }

        public void ConfigureFor(NavigationPage navigationPage)
        {
            _navigationPage = navigationPage;
            ChangeTheme(IsDarkTheme);
        }
    }
}