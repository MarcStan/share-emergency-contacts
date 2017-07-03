using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class EditProfileViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly Action<EmergencyProfile> _onOk;
        private ProfileViewModel _selected;

        public EditProfileViewModel(INavigationService navigationService, EmergencyProfile toEdit, Action<EmergencyProfile> onOk)
        {
            _navigationService = navigationService;
            _onOk = onOk;
            Selected = new ProfileViewModel(toEdit, null);
            SaveCommand = new Command(Ok);
        }

        public ProfileViewModel Selected
        {
            get => _selected;
            private set
            {
                if (Equals(value, _selected)) return;
                _selected = value;
                NotifyOfPropertyChange(nameof(Selected));
            }
        }

        public ICommand SaveCommand { get; }

        public async void Ok()
        {
            _onOk(Selected.Actual);
            await _navigationService.GoBackAsync();

        }
    }
}