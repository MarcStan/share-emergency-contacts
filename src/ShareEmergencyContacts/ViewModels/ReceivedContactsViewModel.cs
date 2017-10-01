using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.ViewModels.Base;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
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

        public async void ScanNewContact()
        {
            var permCheck = IoC.Get<ICheckPermissions>();
            var grantResult = await permCheck.GrantPermissionAsync(PermissionType.Camera);
            switch (grantResult)
            {
                case PermissionResult.Granted:
                    var vm = new ScanCodeViewModel(async p => await AddAsync(p));
                    Device.BeginInvokeOnMainThread(() => _navigationService.NavigateToInstanceAsync(vm));
                    break;
                case PermissionResult.Denied:
                    break;
                case PermissionResult.AlwaysDenied:
                    // user won't even be prompted anymore with a dialog
                    // since this is the main feature of the app this will confuse any user who accidently set "never ask again"
                    // therefore tell him how to fix it
                    var dia = IoC.Get<IUserDialogs>();
                    string navPath;
                    var appName = "Share emergency contacts";
                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:
                            navPath = $"Apps -> {appName} -> Permissions";
                            break;
                        case Device.UWP:
                            navPath = "Privacy -> Camera";
                            break;
                        case Device.iOS:
                            navPath = "Privacy -> Camera";
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported platform '{Device.RuntimePlatform}'.");
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        dia.Alert($"You have permanently denied access to the camera previously. To use this feature again, please go to 'Settings -> {navPath}' and manually enable camera access.", "Camera access denied");
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void ProfileSelected(ProfileViewModel profile)
        {
            if (profile == null)
                return;

            Device.BeginInvokeOnMainThread(async () =>
            {
                var vm = new ProfileVisualizerViewModel(profile, async p =>
                {
                    var r = await ConfirmDelete(p);
                    if (r)
                        Device.BeginInvokeOnMainThread(() => _navigationService.GoBackToRootAsync());
                }, null);
                await _navigationService.NavigateToInstanceAsync(vm);
            });
        }
    }
}