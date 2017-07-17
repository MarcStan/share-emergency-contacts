using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
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
        private BindableCollection<PhoneNumberViewModel> _phoneNumbers;
        private bool _noBirthday;

        public ContactViewModel(EmergencyContact profile, bool displayInsuranceNumber, bool isChild, Action<ContactViewModel> delete)
        {
            CanHaveInsuranceNumber = displayInsuranceNumber;
            CanDelete = delete != null;
            _isChild = isChild;
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            PhoneNumbers = new BindableCollection<PhoneNumberViewModel>(profile.PhoneNumbers.Select(p => new PhoneNumberViewModel(p, FormattedName, async phone =>
            {
                var dia = IoC.Get<IUserDialogs>();
                if (await dia.ConfirmAsync($"Really remove '{phone.Number}'?", "Confirm delete", "Yes", "No"))
                {
                    PhoneNumbers.Remove(phone);
                    NotifyOfPropertyChange(nameof(PhoneNumbers));
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
            NoBirthday = !profile.BirthDate.HasValue;
            AddBirthdayCommand = new Command(() =>
            {
                NoBirthday = false;
            });
            DeleteBirthdayCommand = new Command(() =>
            {
                NoBirthday = true;
            });
            AddNumberCommand = new Command(AddNumber);
        }

        public ICommand AddNumberCommand { get; }

        public string ProfileName
        {
            get => _profile.ProfileName;
            set
            {
                if (value == ProfileName) return;
                _profile.ProfileName = value;
                NotifyOfPropertyChange(nameof(ProfileName));
            }
        }

        public bool NoBirthday
        {
            get => _noBirthday;
            set
            {
                if (value == _noBirthday) return;
                _noBirthday = value;
                if (NoBirthday)
                    _profile.BirthDate = null;
                NotifyOfPropertyChange(nameof(NoBirthday));
                NotifyOfPropertyChange(nameof(ActualBirthDate));
            }
        }

        public ICommand AddBirthdayCommand { get; }

        public ICommand DeleteBirthdayCommand { get; }

        public bool HasBirthday => CanHaveInsuranceNumber == false;

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

        public string FormattedName
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

        public string FirstName
        {
            get => _profile.FirstName;
            set
            {
                if (value == FirstName) return;
                _profile.FirstName = value;
                NotifyOfPropertyChange(nameof(FirstName));
                NotifyOfPropertyChange(nameof(FormattedName));
                NotifyOfPropertyChange(nameof(ProfileName));
            }
        }

        public string LastName
        {
            get => _profile.LastName;
            set
            {
                if (value == LastName) return;
                _profile.LastName = value;
                NotifyOfPropertyChange(nameof(LastName));
                NotifyOfPropertyChange(nameof(FormattedName));
                NotifyOfPropertyChange(nameof(ProfileName));
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

        public DateTime ActualBirthDate
        {
            get => _profile.BirthDate ?? new DateTime(1970, 1, 1);
            set
            {
                if (ActualBirthDate == value) return;
                _profile.BirthDate = value;
                NotifyOfPropertyChange(nameof(ActualBirthDate));
            }
        }

        public string Address
        {
            get => _profile.Address;
            set
            {
                if (Address == value) return;
                _profile.Address = value;
                NotifyOfPropertyChange(nameof(Address));
            }
        }

        public string Note
        {
            get => _profile.Note;
            set
            {
                if (Note == value) return;
                _profile.Note = value;
                NotifyOfPropertyChange(nameof(Note));
            }
        }

        public string InsuranceNumber
        {
            get => CanHaveInsuranceNumber ? _profile.InsuranceNumber : null;
            set
            {
                if (InsuranceNumber == value) return;
                _profile.InsuranceNumber = value;
                NotifyOfPropertyChange(nameof(InsuranceNumber));
            }
        }

        public BindableCollection<PhoneNumberViewModel> PhoneNumbers
        {
            get => _phoneNumbers;
            set
            {
                if (Equals(value, _phoneNumbers)) return;
                _phoneNumbers = value;
                NotifyOfPropertyChange(nameof(PhoneNumbers));
            }
        }

        public void AddNumber()
        {
            var vm = new EditPhoneNumberViewModel(null, num =>
            {
                PhoneNumbers.Add(new PhoneNumberViewModel(num,
                    FormattedName, async phone =>
                    {
                        var dia = IoC.Get<IUserDialogs>();
                        if (await dia.ConfirmAsync($"Really remove '{phone.Number}'?", "Confirm delete", "Yes", "No"))
                        {
                            PhoneNumbers.Remove(phone);
                            NotifyOfPropertyChange(nameof(PhoneNumbers));
                        }
                    }));
            });
            var nav = IoC.Get<INavigationService>();
            nav.NavigateToInstanceAsync(vm);
        }
    }
}