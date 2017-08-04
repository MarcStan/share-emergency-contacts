using Caliburn.Micro;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Helpers
{
    /// <summary>
    /// Helper class to wrap global app settings.
    /// </summary>
    public static class AppSettings
    {
        public static bool IsDarkTheme
        {
            get => Application.Current.Properties.ContainsKey("theme")
                ? "dark".Equals(Application.Current.Properties["theme"]?.ToString(), StringComparison.OrdinalIgnoreCase)
                : IsSystemDefaultThemeDark;
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

        private static bool IsSystemDefaultThemeDark => IoC.Get<IAppInfoProvider>().SystemThemeIsDark;

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

        public static void ConfigureTheme(bool useDark)
        {
            // TODO: implement whenever the fuck xamarin decides to get their shit together qnd releases a non-half-assed-solution that actually works and doesn't crash on half the platforms
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
        public static async Task LoadAsync()
        {
            ConfigureTheme(IsDarkTheme);
            await ConfigureMobileCenterAsync(AllowAnalytics);
        }
    }
}