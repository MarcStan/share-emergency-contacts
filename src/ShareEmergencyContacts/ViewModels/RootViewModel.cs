using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class RootViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public RootViewModel()
        {
            _navigationService = IoC.Get<INavigationService>();
            MyProfilesViewModel = new MyProfilesViewModel(_navigationService);
            ReceivedContactsViewModel = new ReceivedContactsViewModel(_navigationService);

            AboutCommand = new Command(About);
        }

        public MyProfilesViewModel MyProfilesViewModel { get; set; }

        public ReceivedContactsViewModel ReceivedContactsViewModel { get; set; }

        public ICommand AboutCommand { get; }

        public void About()
        {
            _navigationService.NavigateToViewModelAsync<AboutViewModel>();
        }
    }
}
