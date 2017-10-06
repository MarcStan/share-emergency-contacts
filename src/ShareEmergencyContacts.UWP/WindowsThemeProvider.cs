using Windows.UI.Xaml;
using Xamarin.Forms;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsThemeProvider : IThemeProvider
    {
        private NavigationPage _navigationPage;

        public void ChangeTheme(bool darkTheme)
        {
            if (Window.Current.Content is FrameworkElement el)
            {
                el.RequestedTheme = darkTheme ? ElementTheme.Dark : ElementTheme.Light;
                MainPage.Instance.RequestedTheme = el.RequestedTheme;

                // text
                _navigationPage.BarTextColor = darkTheme ? Color.White : Color.Black;
                // bar using colors that uwp provides when forcing RequestedTheme=Dark|Light on App.xaml
                _navigationPage.BarBackgroundColor = Color.FromHex(darkTheme ? "#2B2B2B" : "#F2F2F2");
            }
            // would be nice to use uwp system color, but xamarin doesn't refresh existing pages so we then have new pages with new colors and old pages with old colors..
            // var color = (Windows.UI.Color)Application.Current.Resources["SystemAccentColor"];
            // var cc = Color.FromRgb(color.R, color.G, color.B);

            // use app specific theme color instead
            Color.SetAccent(Color.FromHex("#448AFF"));
        }

        public void ConfigureFor(NavigationPage navigationPage)
        {
            _navigationPage = navigationPage;
        }
    }
}