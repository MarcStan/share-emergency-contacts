using Caliburn.Micro;

namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Version = $"Share emergency contacts v{IoC.Get<IAppInfoProvider>().UserFriendlyVersion}";
        }

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }
    }
}
