using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Linq;
using System.Windows.Input;
using ShareEmergencyContacts.ViewModels.Base;
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

        public async void AddNewProfile()
        {
            var vm = new EditProfileViewModel(_navigationService, null, Add);
            await _navigationService.NavigateToInstanceAsync(vm);
        }

        protected override void ProfileSelected(EmergencyProfile profile)
        {
            if (profile == null)
                return;

            var match = ExistingContacts.FirstOrDefault(c => c.Actual == profile);
            if (match == null)
                return;

            // display barcode directly on my own profiles as user most likely wants to share
            var vm = new ProfileVisualizerViewModel(match, async p =>
            {
                var dia = IoC.Get<IUserDialogs>();
                var r = await dia.ConfirmAsync($"Really delete profile '{profile.ProfileName}'?", "Really delete?", "Yes", "No");
                if (!r)
                    return;

                Delete(p);
                await _navigationService.GoBackToRootAsync();
            }, Edit);
            _navigationService.NavigateToInstanceAsync(vm);
        }

        public async void Edit(EmergencyProfile profile)
        {
            var vm = new EditProfileViewModel(_navigationService, profile, async editedProfile =>
            {
                var storage = IoC.Get<IStorageContainer>();
                await storage.SaveProfileAsync(profile);

                // update UI by just reinserting the item
                var idx = ExistingContacts.IndexOf(ExistingContacts.FirstOrDefault(c => c.Actual == profile));
                if (idx == -1)
                    throw new NotSupportedException("Item not found");

                ExistingContacts.RemoveAt(idx);
                ExistingContacts.Insert(idx, new ProfileViewModel(profile, Delete));
                var dia = IoC.Get<IUserDialogs>();
                dia.Toast("Profile updated!");
            });
            await _navigationService.NavigateToInstanceAsync(vm);
        }
    }
}
