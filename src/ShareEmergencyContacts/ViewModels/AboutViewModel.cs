using Caliburn.Micro;
using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : Screen
    {
        private BindableCollection<string> _poweredBy;
        private const string Url = "https://marcstan.net/betas/SEC";

        public AboutViewModel()
        {
            Version = $"{GetType().GetTypeInfo().Assembly.GetName().Version.ToString(3)}";
            WebsiteCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(Url));
            });

            ShareBetaCommand = new Command(() =>
            {
                var share = IoC.Get<IShareProvider>();
                share.ShareUrl(Url, "Download \"Share emergency contacts\"", "Check out the app \"Share emergency contacts\" (currently in beta)");
            });

            PoweredBy = new BindableCollection<string>
            {
                "Xamarin.Forms",
                "XLabs",
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

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }

        public ICommand WebsiteCommand { get; }

        public ICommand ShareBetaCommand { get; }
    }
}
