using Xamarin.Forms;

namespace ShareEmergencyContacts
{
    public interface IThemeProvider
    {
        void ChangeTheme(bool darkTheme);

        /// <summary>
        /// Some platforms (android) do not change their title bar color with theme.
        /// So we force override on nav page.
        /// https://stackoverflow.com/questions/36062495/xamarin-forms-how-to-change-the-nav-bar-color
        /// </summary>
        /// <param name="navigationPage"></param>
        void ConfigureFor(NavigationPage navigationPage);

        /// <summary>
        /// Returns whether user has currenty enabled dark theme.
        /// </summary>
        bool IsDarkTheme { get; }
    }
}