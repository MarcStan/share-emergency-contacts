using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.Base;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
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
            EditProfileViewModel vm = null;
            vm = new EditProfileViewModel(null, () => Add(vm.Selected.Actual));
            _navigationService.NavigateToInstanceAsync(vm);
        }

        protected override void ProfileSelected(EmergencyProfile profile)
        {
            if (profile == null)
                return;

            var match = ExistingContacts.FirstOrDefault(c => c.Actual == profile);
            if (match == null)
                return;

            // display barcode directly on my own profiles as user most likely wants to share
            var vm = new ProfileVisualizerViewModel(match, Delete, Edit);
            _navigationService.NavigateToInstanceAsync(vm);
        }

        public void Edit(EmergencyProfile profile)
        {
            EditProfileViewModel vm = null;
            var originalName = profile.ProfileName;
            vm = new EditProfileViewModel(profile, () =>
            {
                UpdateEdited(vm.Selected.Actual, originalName);
            });
            _navigationService.NavigateToInstanceAsync(vm);
        }

        public async void Delete(EmergencyProfile p)
        {
            var r = await ConfirmDelete(p);
            if (r)
                Device.BeginInvokeOnMainThread(() => _navigationService.GoBackToRootAsync());
        }

        private async void UpdateEdited(EmergencyProfile profile, string originalName)
        {
            var storage = IoC.Get<IStorageContainer>();
            if (originalName != profile.ProfileName)
            {
                // delete old file, write new
                // just needs a dummy profile to infer the filename from ProfileName property
                var mock = new EmergencyProfile { ProfileName = originalName };
                await storage.DeleteProfileAsync(mock);
            }
            await storage.SaveProfileAsync(profile);

            // update UI by just reinserting the item
            var idx = ExistingContacts.IndexOf(ExistingContacts.FirstOrDefault(c => c.Actual == profile));
            if (idx == -1)
                throw new NotSupportedException("Item not found");

            ExistingContacts.RemoveAt(idx);
            var pr = new ProfileViewModel(profile, Delete);
            ExistingContacts.Insert(idx, pr);
            var dia = IoC.Get<IUserDialogs>();
            await _navigationService.GoBackToRootAsync();
            await _navigationService.NavigateToInstanceAsync(new ProfileVisualizerViewModel(pr, Delete, Edit));
            dia.Toast("Profile updated!");
        }
    }
}
