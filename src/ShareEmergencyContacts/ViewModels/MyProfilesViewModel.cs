using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.Base;
using ShareEmergencyContacts.ViewModels.ForModels;
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
            EditProfileViewModel vm = null;
            vm = new EditProfileViewModel(null, async () => await Add(vm.Selected.Actual));
            Device.BeginInvokeOnMainThread(() => _navigationService.NavigateToInstanceAsync(vm));
        }

        protected override void ProfileSelected(ProfileViewModel profile)
        {
            if (profile == null)
                return;

            // display barcode directly on my own profiles as user most likely wants to share
            var vm = new ProfileVisualizerViewModel(profile, Delete, Edit, true);
            Device.BeginInvokeOnMainThread(() => _navigationService.NavigateToInstanceAsync(vm));
        }

        public void Edit(ProfileViewModel profile)
        {
            EditProfileViewModel vm = null;
            var originalName = profile.ProfileName;
            vm = new EditProfileViewModel(profile, () =>
            {
                UpdateEdited(vm.Selected, originalName);
            });
            Device.BeginInvokeOnMainThread(() => _navigationService.NavigateToInstanceAsync(vm));
        }

        public async void Delete(ProfileViewModel p)
        {
            var r = await ConfirmDelete(p);
            if (r)
                Device.BeginInvokeOnMainThread(() => _navigationService.GoBackToRootAsync());
        }

        private async void UpdateEdited(ProfileViewModel profile, string originalName)
        {
            var storage = IoC.Get<IStorageContainer>();
            if (originalName != profile.ProfileName)
            {
                // delete old file, write new
                // just needs a dummy profile to infer the filename from ProfileName property
                var mock = new EmergencyProfile { ProfileName = originalName };
                await storage.DeleteProfileAsync(mock);
            }
            await storage.SaveProfileAsync(profile.Actual);

            var dia = IoC.Get<IUserDialogs>();
            dia.Toast("Profile updated!");
        }
    }
}
