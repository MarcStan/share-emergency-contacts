using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Version = $"Share emergency contacts v{IoC.Get<IAppInfoProvider>().UserFriendlyVersion}";
            CloseCommand = new Command(Close);
        }

        public ICommand CloseCommand { get; }

        /// <summary>
        /// Returns the current version
        /// </summary>
        public string Version { get; }

        public void Close()
        {
            IoC.Get<INavigationService>().GoBackAsync();
        }
    }
}
