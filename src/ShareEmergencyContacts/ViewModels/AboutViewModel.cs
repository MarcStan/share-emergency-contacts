using Caliburn.Micro;
using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : Screen
    {
        private bool _betaOverlay;

        public AboutViewModel()
        {
            Version = $"{GetType().GetTypeInfo().Assembly.GetName().Version.ToString(3)}";
            WebsiteCommand = new Command(() =>
            {
                Device.OpenUri(new Uri("https://marcstan.net/"));
            });
            ShareBetaCommand = new Command(() =>
            {
                var share = IoC.Get<IShareProvider>();
                share.ShareUrl("https://marcstan.net/private/betas/ShareEmergencyContacts/", "Download \"Share emergency contacts\"", "Check out the app \"Share emergency contacts\" (currently in beta)");
            });
        }

        public bool BetaOverlay
        {
            get => _betaOverlay;
            set
            {
                if (value == _betaOverlay) return;
                _betaOverlay = value;
                NotifyOfPropertyChange(nameof(BetaOverlay));
            }
        }

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }

        public ICommand WebsiteCommand { get; }

        public ICommand ShareBetaCommand { get; }
    }
}
