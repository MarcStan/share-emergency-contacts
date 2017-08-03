using Acr.UserDialogs;
using Caliburn.Micro;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using ShareEmergencyContacts.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class EditProfileViewModel : Screen
    {
        private readonly Func<Task> _save;
        private ProfileViewModel _selected;

        public EditProfileViewModel(ProfileViewModel toEdit, Func<Task> save)
        {
            _save = save;
            // TODO: this is ugly, we don't provide rename/delete functors because we rely on the fact that the caller will scrap the instance on save (taking only .Actual) and instead creates a new wrapper with the correct functors
            Selected = toEdit ?? new ProfileViewModel(new EmergencyProfile(), null, null);
            SaveCommand = new Command(Save);
            AddEmergencyContactCommand = new Command(AddEmergencyContact);
            AddInsuranceContactCommand = new Command(AddInsuranceContact);
        }

        public void CancelBackButton(EditProfileView.BackButtonHelper h)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var dia = IoC.Get<IUserDialogs>();
                if (await dia.ConfirmAsync("Do you really want to discard your changes?", "Discard changes?", "Yes", "No"))
                {
                    // stay on the page
                    h.SetResult(true);
                    return;
                }
                h.SetResult(false);
            });
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

        public async void Save()
        {
            if (!CanSave(out string message))
            {
                IoC.Get<IUserDialogs>().Alert(message);
                return;
            }
            await _save();
        }

        private bool CanSave(out string message)
        {
            if (Selected.Actual.IsBlank())
            {
                message = "Please add at least one detail on the profile!";
                return false;
            }
            foreach (var e in Selected.EmergencyContacts)
            {
                if (string.IsNullOrWhiteSpace(e.FirstName) && string.IsNullOrWhiteSpace(e.LastName))
                {
                    message = "Emergency contact must have either first or last name!";
                    return false;
                }
                foreach (var p in e.PhoneNumbers)
                {
                    if (string.IsNullOrWhiteSpace(p.Number))
                    {
                        message = "Phone number cannot be empty!";
                        return false;
                    }
                }
            }
            foreach (var i in Selected.InsuranceContacts)
            {
                if (string.IsNullOrWhiteSpace(i.FirstName) && string.IsNullOrWhiteSpace(i.LastName))
                {
                    message = "Insurance contact must have either first or last name!";
                    return false;
                }
                foreach (var p in i.PhoneNumbers)
                {
                    if (string.IsNullOrWhiteSpace(p.Number))
                    {
                        message = "Phone number cannot be empty!";
                        return false;
                    }
                }
            }
            foreach (var p in Selected.PhoneNumbers)
            {
                if (string.IsNullOrWhiteSpace(p.Number))
                {
                    message = "Phone number cannot be empty!";
                    return false;
                }
            }
            message = null;
            return true;
        }

        private void AddEmergencyContact()
        {
            Analytics.TrackEvent(AnalyticsEvents.AddEmergencyContact);
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
            Analytics.TrackEvent(AnalyticsEvents.AddInsuranceContact);
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