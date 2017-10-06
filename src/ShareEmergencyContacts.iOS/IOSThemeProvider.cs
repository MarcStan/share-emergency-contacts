using Xamarin.Forms;

namespace ShareEmergencyContacts.iOS
{
    public class IOSThemeProvider : IThemeProvider
    {
        private NavigationPage _navigationPage;

        public bool IsDarkTheme { get; }

        public void ChangeTheme(bool darkTheme)
        {
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