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
        private BindableCollection<string> _poweredBy;
        private bool _allowAnalytics;
        private bool _useDarkTheme;
        private const string Url = "https://marcstan.net/betas/SEC";
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

            PoweredBy = new BindableCollection<string>
            {
                "Xamarin.Forms",
                "Caliburn.Micro",
                "Acr UserDialogs",
                "ZXing .Net Mobile Forms"
            };
        }

        public BindableCollection<string> PoweredBy
        {
            get => _poweredBy;
            set
            {
                if (Equals(value, _poweredBy)) return;
                _poweredBy = value;
                NotifyOfPropertyChange(nameof(PoweredBy));
            }
        }

        public bool UseDarkTheme
        {
            get => Application.Current.Properties.ContainsKey("theme") ? (Application.Current.Properties["theme"] == "dark") : IsSystemDefaultThemeDark();
            set
            {
                if (value == _useDarkTheme) return;
                _useDarkTheme = value;
                Application.Current.Properties["theme"] = UseDarkTheme ? "dark" : "light";
                NotifyOfPropertyChange(nameof(UseDarkTheme));
            }
        }

        private bool IsSystemDefaultThemeDark()
        {
            throw new NotSupportedException();
        }

        public bool AllowAnalytics
        {
            get => _allowAnalytics;
            set
            {
                if (value == _allowAnalytics) return;
                _allowAnalytics = value;
                NotifyOfPropertyChange(nameof(AllowAnalytics));
                NotifyOfPropertyChange(nameof(AnalyticsMessage));
            }
        }

        public string AnalyticsMessage => AllowAnalytics
            ? "These reports help me improve the app by spotting crashes early on and fixing them asap. " + Environment.NewLine +
              "All data uploaded is totally anonymous and only consists of simple events such as 'Edit profile clicked' or 'open barcode scanner' (NONE OF YOUR ENTERED DATA IS EVER UPLOADED)."
            : "I respect your privacy." + Environment.NewLine +
              "No usage or crash reports are being uploaded. Only update checks will be made whenever the app is opened.";

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }

        public ICommand WebsiteCommand { get; }

        public ICommand PrivacyCommand { get; }

        public ICommand ShareBetaCommand { get; }
    }
}
