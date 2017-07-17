using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class EditProfileViewModel : Screen
    {
        private readonly Action<EmergencyProfile> _onOk;
        private ProfileViewModel _selected;

        public EditProfileViewModel(EmergencyProfile toEdit, Action<EmergencyProfile> onOk)
        {
            _onOk = onOk;
            toEdit = toEdit ?? new EmergencyProfile();
            Selected = new ProfileViewModel(toEdit, null);
            SaveCommand = new Command(Ok);
            AddEmergencyContactCommand = new Command(AddEmergencyContact);
            AddInsuranceContactCommand = new Command(AddInsuranceContact);
        }

        public ICommand AddEmergencyContactCommand { get; }

        public ICommand AddInsuranceContactCommand { get; }

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

        public void Ok()
        {
            _onOk(Selected.Actual);

            IoC.Get<INavigationService>().GoBackAsync();
        }

        private void AddEmergencyContact()
        {
            Selected.EmergencyContacts.Add(new ContactViewModel(new EmergencyContact(), false, true, p =>
            {
                Selected.EmergencyContacts.Remove(p);
                NotifyOfPropertyChange(nameof(Selected));
            }));
            NotifyOfPropertyChange(nameof(Selected));
        }

        private void AddInsuranceContact()
        {
            Selected.InsuranceContacts.Add(new ContactViewModel(new EmergencyContact(), true, true, p =>
            {
                Selected.InsuranceContacts.Remove(p);
                NotifyOfPropertyChange(nameof(Selected));
            }));
            NotifyOfPropertyChange(nameof(Selected));
        }
    }
}