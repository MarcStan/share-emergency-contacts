using Caliburn.Micro;
using ShareEmergencyContacts.Helpers;
using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : Screen
    {
        private const string Url = "https://marcstan.net/apps/sec/";
        private const string PrivacyUrl = "https://marcstan.net/apps/sec/privacy/";

        public AboutViewModel()
        {
            Version = $"{GetType().GetTypeInfo().Assembly.GetName().Version.ToString(3)}";
            WebsiteCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(Url));
            });
            PrivacyCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(PrivacyUrl));
            });

            ShareCommand = new Command(() =>
            {
                var share = IoC.Get<IShareProvider>();
                share.ShareUrl(Url, "Download \"Share emergency contacts\"",
                    "Check out the app \"Share emergency contacts\"" +
                    ("true".Equals(Application.Current.Resources["IsInBeta"] as string,
                        StringComparison.OrdinalIgnoreCase)
                        ? " (currently in beta)"
                        : ""));
            });
        }

        public string InviteText { get; } = "true".Equals(Application.Current.Resources["IsInBeta"] as string, StringComparison.OrdinalIgnoreCase)
            ? "Invite a friend to the beta"
            : "Invite a friend to use the app";

        public bool UseDarkTheme
        {
            get => AppSettings.IsDarkTheme;
            set
            {
                if (value == UseDarkTheme) return;
                AppSettings.IsDarkTheme = value;
                NotifyOfPropertyChange(nameof(UseDarkTheme));
            }
        }

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }

        public ICommand WebsiteCommand { get; }

        public ICommand PrivacyCommand { get; }

        public ICommand ShareCommand { get; }
    }
}
