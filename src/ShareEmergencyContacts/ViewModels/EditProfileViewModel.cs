using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System.Windows.Input;
using Xamarin.Forms;
using Action = System.Action;

namespace ShareEmergencyContacts.ViewModels
{
    public class EditProfileViewModel : Screen
    {
        private readonly Action _save;
        private ProfileViewModel _selected;

        public EditProfileViewModel(EmergencyProfile toEdit, Action save)
        {
            _save = save;
            toEdit = toEdit ?? new EmergencyProfile();
            Selected = new ProfileViewModel(toEdit, null);
            SaveCommand = new Command(Save);
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

        public void Save()
        {
            _save();

            IoC.Get<INavigationService>().GoBackAsync();
        }

        private void AddEmergencyContact()
        {
            var contact = new EmergencyContact();
            Selected.Actual.EmergencyContacts.Add(contact);
            Selected.EmergencyContacts.Add(new ContactViewModel(contact, false, true, p =>
            {
                Selected.EmergencyContacts.Remove(p);
                Selected.Actual.EmergencyContacts.Remove(p.Profile);
                NotifyOfPropertyChange(nameof(Selected));
            }));
            NotifyOfPropertyChange(nameof(Selected));
        }

        private void AddInsuranceContact()
        {
            var contact = new EmergencyContact();
            Selected.Actual.InsuranceContacts.Add(contact);
            Selected.InsuranceContacts.Add(new ContactViewModel(contact, true, true, p =>
            {
                Selected.InsuranceContacts.Remove(p);
                Selected.Actual.InsuranceContacts.Remove(p.Profile);
                NotifyOfPropertyChange(nameof(Selected));
            }));
            NotifyOfPropertyChange(nameof(Selected));
        }
    }
}