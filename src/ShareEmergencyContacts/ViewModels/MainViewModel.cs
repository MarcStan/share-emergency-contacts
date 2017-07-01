using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models.Data;
using System.Linq;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// Lists received contacts.
    /// </summary>
    public class MainViewModel : ProfileListViewModel
    {
        private readonly INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService) : base(false)
        {
            _navigationService = navigationService;
        }

        public void ScanNewContact()
        {
            _navigationService.NavigateToViewModelAsync<ScanCodeViewModel>();
        }

        public void ShareMyDetails()
        {
            _navigationService.NavigateToViewModelAsync<MyProfilesViewModel>();
        }

        protected override void ProfileSelected(EmergencyProfile profile)
        {
            if (profile == null)
                return;

            var match = ExistingContacts.FirstOrDefault(c => c.Actual == profile);
            if (match == null)
                return;

            var vm = new SelectedProfileViewModel(match);
            _navigationService.NavigateToInstanceAsync(vm);
        }
    }
}