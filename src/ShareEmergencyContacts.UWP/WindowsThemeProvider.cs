using Windows.UI.Xaml;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsThemeProvider : IThemeProvider
    {
        public void ChangeTheme(bool darkTheme)
        {
            if (Window.Current.Content is FrameworkElement el)
            {
                el.RequestedTheme = darkTheme ? ElementTheme.Dark : ElementTheme.Light;
                MainPage.Instance.RequestedTheme = el.RequestedTheme;
            }

        }
    }
}