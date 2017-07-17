using Caliburn.Micro;
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

        public override void CanClose(Action<bool> callback)
        {
            // this is never called when back button is used
            // supposedly this is not good practice on iOS anyway

            //// ask user to confirm abortion
            //var dia = IoC.Get<IUserDialogs>();
            //var r = await dia.ConfirmAsync($"Do you really want to cancel profile {(_creatingNew ? "creation" : "editing")}? All changes will be lost.", "Confirm abort", "Yes", "No");
            //if (!r)
            //{
            //    callback(false);
            //    base.CanClose(callback);
            //    return;
            //}
            //base.CanClose(callback);
        }

        public void Ok()
        {
            _onOk(Selected.Actual);
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