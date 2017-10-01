using Caliburn.Micro;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : Screen
    {
        private const string Url = "https://marcstan.net/betas/SEC/";
        private const string PrivacyUrl = "https://marcstan.net/privacy/betas/SEC";

        public AboutViewModel()
        {
            Version = $"{GetType().GetTypeInfo().Assembly.GetName().Version.ToString(3)}";
            WebsiteCommand = new Command(() =>
            {
                Analytics.TrackEvent(AnalyticsEvents.OpenWebsite);
                Device.OpenUri(new Uri(Url));
            });
            PrivacyCommand = new Command(() =>
            {
                Analytics.TrackEvent(AnalyticsEvents.OpenPrivacy);
                Device.OpenUri(new Uri(PrivacyUrl));
            });

            ShareBetaCommand = new Command(() =>
            {
                var share = IoC.Get<IShareProvider>();
                Analytics.TrackEvent(AnalyticsEvents.ShareApp);
                share.ShareUrl(Url, "Download \"Share emergency contacts\"", "Check out the app \"Share emergency contacts\" (currently in beta)");
            });
        }

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

        public bool AllowAnalytics
        {
            get => AppSettings.AllowAnalytics;
            set
            {
                if (value == AllowAnalytics) return;
                AppSettings.AllowAnalytics = value;
                NotifyOfPropertyChange(nameof(AllowAnalytics));
                NotifyOfPropertyChange(nameof(AnalyticsMessage));
            }
        }

        public string AnalyticsMessage => AllowAnalytics
            ? "These reports help me improve the app by spotting crashes early on. " + Environment.NewLine +
              "Only anonymous data is collected." + Environment.NewLine +
              "None of the text you enter is ever uploaded."
            : "I respect your privacy." + Environment.NewLine +
              "No usage or crash reports will be uploaded. Only update checks will be made whenever the app is opened.";

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }

        public ICommand WebsiteCommand { get; }

        public ICommand PrivacyCommand { get; }

        public ICommand ShareBetaCommand { get; }
    }
}
