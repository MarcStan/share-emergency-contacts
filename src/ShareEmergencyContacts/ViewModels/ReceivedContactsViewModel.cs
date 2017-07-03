using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models.Data;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// Lists received contacts.
    /// </summary>
    public class ReceivedContactsViewModel : ProfileListViewModel
    {
        private readonly INavigationService _navigationService;

        public ReceivedContactsViewModel(INavigationService navigationService) : base(false)
        {
            _navigationService = navigationService;
            ScanCommand = new Command(ScanNewContact);
        }

        public ICommand ScanCommand { get; }

        public void ScanNewContact()
        {
            var vm = new ScanCodeViewModel(_navigationService, Add);
            _navigationService.NavigateToInstanceAsync(vm);
        }

        protected override void ProfileSelected(EmergencyProfile profile)
        {
            if (profile == null)
                return;

            var match = ExistingContacts.FirstOrDefault(c => c.Actual == profile);
            if (match == null)
                return;

            var vm = new ProfileVisualizerViewModel(_navigationService, match);
            _navigationService.NavigateToInstanceAsync(vm);
        }
    }
}