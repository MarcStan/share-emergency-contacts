using Caliburn.Micro;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace ShareEmergencyContacts.Helpers
{
    /// <summary>
    /// Helper class to wrap global app settings.
    /// </summary>
    public static class AppSettings
    {
        public static bool IsDarkTheme
        {
            get
            {
                // default to light theme if not found
                return Application.Current.Properties.ContainsKey("theme") && "dark".Equals(
                           Application.Current.Properties["theme"]?.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                Application.Current.Properties["theme"] = value ? "dark" : "light";
                Task.Run(async () =>
                {
                    ConfigureTheme(IsDarkTheme);
                    await Application.Current.SavePropertiesAsync();
                });
            }
        }

        public static void ConfigureTheme(bool useDark, bool delegateToMainThread = true)
        {
            var exec = new Action(() =>
            {
                IoC.Get<IThemeProvider>().ChangeTheme(useDark);
            });

            if (delegateToMainThread)
                Device.BeginInvokeOnMainThread(exec);
            else
                exec();
        }

        public static void LoadTheme()
        {
            ConfigureTheme(IsDarkTheme, false);
        }
    }
}