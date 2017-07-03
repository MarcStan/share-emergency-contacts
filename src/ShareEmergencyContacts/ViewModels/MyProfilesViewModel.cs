using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models.Data;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// Lists my own profiles.
    /// </summary>
    public class MyProfilesViewModel : ProfileListViewModel
    {
        private readonly INavigationService _navigationService;

        public MyProfilesViewModel(INavigationService navigationService) : base(true)
        {
            _navigationService = navigationService;
            AddCommand = new Command(AddNewProfile);
        }

        public ICommand AddCommand { get; }

        public void AddNewProfile()
        {
            _navigationService.NavigateToViewModelAsync<EditProfileViewModel>();
        }

        protected override void ProfileSelected(EmergencyProfile profile)
        {
            if (profile == null)
                return;

            var match = ExistingContacts.FirstOrDefault(c => c.Actual == profile);
            if (match == null)
                return;

            // display barcode directly on my own profiles as user most likely wants to share
            var vm = new SelectedProfileViewModel(match, true, true);
            _navigationService.NavigateToInstanceAsync(vm);
        }
    }
}
