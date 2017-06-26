namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel(IAppInfoProvider provider)
        {
            Version = $"Share emergency contacts v{provider.UserFriendlyVersion}";
        }

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }
    }
}
