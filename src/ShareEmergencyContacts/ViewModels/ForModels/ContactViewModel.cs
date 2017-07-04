using Acr.UserDialogs;
using Caliburn.Micro;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels.ForModels
{
    /// <summary>
    /// Wrapper for the <see cref="EmergencyContact"/> to display all its data.
    /// </summary>
    public class ContactViewModel : PropertyChangedBase
    {
        private bool _isEditable;

        private readonly bool _isChild;
        private readonly EmergencyContact _profile;

        public ContactViewModel(EmergencyContact profile, bool displayInsuranceNumber, bool isChild, Action<ContactViewModel> delete)
        {
            CanHaveInsuranceNumber = displayInsuranceNumber;
            CanDelete = delete != null;
            _isChild = isChild;
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            PhoneNumbers = new BindableCollection<PhoneNumberViewModel>(profile.PhoneNumbers.Select(p => new PhoneNumberViewModel(p, Name, async phone =>
            {
                var dia = IoC.Get<IUserDialogs>();
                if (await dia.ConfirmAsync($"Really remove '{phone.Number}'?", "Confirm delete", "Yes", "No"))
                {
                    PhoneNumbers.Remove(phone);
                }
            })));

            DeleteContactCommand = new Command(async () =>
            {
                var dia = IoC.Get<IUserDialogs>();
                if (await dia.ConfirmAsync($"Really delete '{ProfileName}'?", "Confirm delete", "Yes", "No"))
                {
                    delete?.Invoke(this);
                }
            });
        }

        public string ProfileName
        {
            get => _profile.ProfileName;
            set
            {
                if (value == ProfileName) return;
                _profile.ProfileName = value;
            }
        }

        public int NameSize => _isChild ? 24 : 16;

        public bool CanHaveInsuranceNumber { get; }

        public bool CanDelete { get; }

        public ICommand DeleteContactCommand { get; }

        public virtual bool IsEditable
        {
            get => _isEditable;
            set
            {
                if (value == _isEditable) return;
                _isEditable = value;
                NotifyOfPropertyChange(nameof(IsEditable));
                foreach (var n in PhoneNumbers)
                {
                    n.IsEditable = IsEditable;
                }
            }
        }

        public string Name
        {
            get
            {
                // try to always return a name
                if (string.IsNullOrWhiteSpace(_profile.FirstName) && string.IsNullOrWhiteSpace(_profile.LastName))
                    return ProfileName;

                if (string.IsNullOrWhiteSpace(_profile.FirstName))
                    return string.IsNullOrWhiteSpace(_profile.LastName) ? null : _profile.LastName;
                if (string.IsNullOrWhiteSpace(_profile.LastName))
                    return string.IsNullOrWhiteSpace(_profile.FirstName) ? null : _profile.FirstName;

                return $"{_profile.FirstName} {_profile.LastName}";
            }
        }

        public string BirthDate
        {
            get
            {
                var b = _profile.BirthDate;
                return b?.ToString("D");
            }
        }

        public string Address => _profile.Address;

        public string Note => _profile.Note;

        public string InsuranceNumber => CanHaveInsuranceNumber ? _profile.InsuranceNumber : null;

        public BindableCollection<PhoneNumberViewModel> PhoneNumbers { get; }
    }
}