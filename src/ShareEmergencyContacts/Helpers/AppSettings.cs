using Caliburn.Micro;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Themes;
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

        public static bool AllowAnalytics
        {
            get => !Application.Current.Properties.ContainsKey("analytics") ||
                   "true".Equals(Application.Current.Properties["analytics"]?.ToString(),
                       StringComparison.OrdinalIgnoreCase);
            set
            {
                var v = value;
                if (v == AllowAnalytics) return;
                Application.Current.Properties["analytics"] = v;
                Task.Run(async () =>
                {
                    await ConfigureMobileCenterAsync(v);
                    await Application.Current.SavePropertiesAsync();
                });
            }
        }

        public static void ConfigureTheme(bool useDark, bool delegateToMainThread = true)
        {
            var exec = new Action(() =>
            {
                App.Current.Resources.MergedWith = useDark ? typeof(DarkThemeResources) : typeof(LightThemeResources);
                IoC.Get<IThemeProvider>().ChangeTheme(useDark);
            });

            if (delegateToMainThread)
                Device.BeginInvokeOnMainThread(exec);
            else
                exec();
        }

        /// <summary>
        /// Enables or disables all mobile center services.
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static async Task ConfigureMobileCenterAsync(bool enabled)
        {
            await Analytics.SetEnabledAsync(enabled);
            await Crashes.SetEnabledAsync(enabled);
            await MobileCenter.SetEnabledAsync(enabled);
        }

        /// <summary>
        /// Sets all settings based on the loaded settings (or defaults).
        /// </summary>
        public static async Task LoadMobileCenterAsync()
        {
            await ConfigureMobileCenterAsync(AllowAnalytics);
        }

        public static void LoadTheme()
        {
            ConfigureTheme(IsDarkTheme, false);
        }
    }
}