using Acr.UserDialogs;
using Caliburn.Micro;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using ShareEmergencyContacts.Views;
using System;
using System.Collections.Specialized;
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

            Selected.TextChanged += TextEntryCompleted;
            Selected.EmergencyContacts.CollectionChanged += CollectionChanged;
            Selected.InsuranceContacts.CollectionChanged += CollectionChanged;

            SaveCommand = new Command(Save);
            AddEmergencyContactCommand = new Command(AddEmergencyContact);
            AddInsuranceContactCommand = new Command(AddInsuranceContact);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ContactViewModel c in e.NewItems)
                {
                    c.TextChanged += TextEntryCompleted;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ContactViewModel c in e.OldItems)
                {
                    c.TextChanged -= TextEntryCompleted;
                }
            }
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
            int total = TotalCharacters;
            if (!CanSave(total, out string message))
            {
                IoC.Get<IUserDialogs>().Alert(message);
                return;
            }
            await _save();
        }

        private bool CanSave(int totalChars, out string message)
        {
            if (totalChars > DataLimits.MaxCharacterCount)
            {
                int chars = totalChars - DataLimits.MaxCharacterCount;
                message = $"Too much content. The QR code won't be able to display it all, please remove at least {chars} characters!";
                return false;
            }
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

        /// <summary>
        /// Gets the total number of characters used by this profile.
        /// </summary>
        public int TotalCharacters
        {
            get
            {
                // only call ToText if we can save, if we can't save it means profile is still invalid (e.g. contact doens't have profile name, etc.
                if (!CanSave(-1, out _))
                    return -1;
                return EmergencyProfile.ToRawText(Selected.Actual).Length;
            }
        }

        public void TextEntryCompleted(object sender, EventArgs e) => TextEntryCompleted();

        public void TextEntryCompleted()
        {
            int curr = TotalCharacters;
            if (curr == -1)
                return;

            int max = DataLimits.MaxCharacterCount;
            const int charsLeftBeforNotify = 256;
            if (curr > max)
            {
                IoC.Get<IUserDialogs>().Toast($"{max} characters exceeded! Cannot save.");
            }
            else if (curr >= max - charsLeftBeforNotify)
            {
                // only notify user when he gets close to the upper limit
                // otherwise we would spam him with "10/1000 chars used" messages
                IoC.Get<IUserDialogs>().Toast($"{curr}/{max} characters");
            }
        }

        private void AddEmergencyContact()
        {
            if (Selected.EmergencyContacts.Count + Selected.InsuranceContacts.Count >= DataLimits.MaxSubContacts)
            {
                Analytics.TrackEvent(AnalyticsEvents.AddEmergencyContactLimitReached);
                Device.BeginInvokeOnMainThread(async () => await IoC.Get<IUserDialogs>().AlertAsync($"Each contact may only have a maximum of {DataLimits.MaxSubContacts} contacts!", "Limit reached", "Ok"));
                return;
            }
            Analytics.TrackEvent(AnalyticsEvents.AddEmergencyContact);
            var contact = new EmergencyContact();
            Selected.Actual.EmergencyContacts.Add(contact);
            Selected.EmergencyContacts.Add(new ContactViewModel(contact, false, true, p =>
            {
                Selected.EmergencyContacts.Remove(p);
                Selected.Actual.EmergencyContacts.Remove(p.Profile);
                NotifyOfPropertyChange(nameof(Selected));
                TextEntryCompleted();
            }));
            NotifyOfPropertyChange(nameof(Selected));
            TextEntryCompleted();
        }

        private void AddInsuranceContact()
        {
            if (Selected.EmergencyContacts.Count + Selected.InsuranceContacts.Count >= DataLimits.MaxSubContacts)
            {
                Analytics.TrackEvent(AnalyticsEvents.AddInsuranceContactLimitReached);
                Device.BeginInvokeOnMainThread(async () => await IoC.Get<IUserDialogs>().AlertAsync($"Each contact may only have a maximum of {DataLimits.MaxSubContacts} contacts!", "Limit reached", "Ok"));
                return;
            }
            Analytics.TrackEvent(AnalyticsEvents.AddInsuranceContact);
            var contact = new EmergencyContact();
            Selected.Actual.InsuranceContacts.Add(contact);
            Selected.InsuranceContacts.Add(new ContactViewModel(contact, true, true, p =>
            {
                Selected.InsuranceContacts.Remove(p);
                Selected.Actual.InsuranceContacts.Remove(p.Profile);
                NotifyOfPropertyChange(nameof(Selected));
                TextEntryCompleted();
            }));
            NotifyOfPropertyChange(nameof(Selected));
            TextEntryCompleted();
        }
    }
}