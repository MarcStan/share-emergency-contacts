using System.Reflection;

namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Version = $"Share emergency contacts v{GetType().GetTypeInfo().Assembly.GetName().Version.ToString(3)}";
        }

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }
    }
}
