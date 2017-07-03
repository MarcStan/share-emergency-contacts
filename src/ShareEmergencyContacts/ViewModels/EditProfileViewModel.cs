using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class EditProfileViewModel : ViewModelBase
    {
        private readonly Action<EmergencyProfile> _onOk;
        private ProfileViewModel _selected;

        public EditProfileViewModel(EmergencyProfile toEdit, Action<EmergencyProfile> onOk)
        {
            _onOk = onOk;
            Selected = new ProfileViewModel(toEdit, null);
            OkCommand = new Command(Ok);
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

        public ICommand OkCommand { get; }

        public void Ok()
        {
            _onOk(Selected.Actual);
        }
    }
}