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
        private readonly bool _isChild;
        private BindableCollection<PhoneNumberViewModel> _phoneNumbers;
        private bool _noBirthday;
        private EmergencyContact _profile;

        public ContactViewModel(EmergencyContact profile, bool displayInsuranceNumber, bool isChild, Action<ContactViewModel> delete)
        {
            CanHaveInsuranceNumber = displayInsuranceNumber;
            CanDelete = delete != null;
            _isChild = isChild;
            IsEmergencyContact = IsSubContact && !CanHaveInsuranceNumber;
            Profile = profile ?? throw new ArgumentNullException(nameof(profile));
            PhoneNumbers = new BindableCollection<PhoneNumberViewModel>(profile.PhoneNumbers.Select(p => new PhoneNumberViewModel(p, FormattedName, async phone =>
            {
                var dia = IoC.Get<IUserDialogs>();
                if (await dia.ConfirmAsync($"Really remove '{phone.Number}'?", "Confirm delete", "Yes", "No"))
                {
                    PhoneNumbers.Remove(phone);
                    profile.PhoneNumbers.Remove(phone.Phone);
                    NotifyOfPropertyChange(nameof(PhoneNumbers));
                }
            })));

            DeleteContactCommand = new Command(async () =>
            {
                var dia = IoC.Get<IUserDialogs>();
                if (await dia.ConfirmAsync($"Really delete '{FormattedName}'?", "Confirm delete", "Yes", "No"))
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

        public bool IsSubContact => _isChild;

        public bool HasRelationship => IsEmergencyContact && !string.IsNullOrEmpty(Relationship);

        public bool IsEmergencyContact { get; }

        public EmergencyContact Profile
        {
            get => _profile;
            set
            {
                if (Equals(value, _profile)) return;
                _profile = value;
                NotifyOfPropertyChange(nameof(Profile));
            }
        }

        public ICommand AddNumberCommand { get; }

        public string ProfileName
        {
            get => Profile.ProfileName;
            set
            {
                if (value == ProfileName) return;
                Profile.ProfileName = value;
                NotifyOfPropertyChange(nameof(ProfileName));
            }
        }

        public string Email
        {
            get => Profile.Email;
            set
            {
                if (value == Email) return;
                Profile.Email = value;
                NotifyOfPropertyChange(nameof(Email));
            }
        }

        public string Relationship
        {
            get => Profile.Relationship;
            set
            {
                if (value == Relationship) return;
                Profile.Relationship = value;
                NotifyOfPropertyChange(nameof(Relationship));
                NotifyOfPropertyChange(nameof(HasRelationship));
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
                    Profile.BirthDate = null;
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

        public string FormattedName
        {
            get
            {
                // try to always return a name
                if (string.IsNullOrWhiteSpace(Profile.FirstName) && string.IsNullOrWhiteSpace(Profile.LastName))
                    return ProfileName;

                if (string.IsNullOrWhiteSpace(Profile.FirstName))
                    return string.IsNullOrWhiteSpace(Profile.LastName) ? null : Profile.LastName;
                if (string.IsNullOrWhiteSpace(Profile.LastName))
                    return string.IsNullOrWhiteSpace(Profile.FirstName) ? null : Profile.FirstName;

                return $"{Profile.FirstName} {Profile.LastName}";
            }
        }

        public string FirstName
        {
            get => Profile.FirstName;
            set
            {
                if (value == FirstName) return;
                Profile.FirstName = value;
                NotifyOfPropertyChange(nameof(FirstName));
                NotifyOfPropertyChange(nameof(FormattedName));
                NotifyOfPropertyChange(nameof(ProfileName));
            }
        }

        public string LastName
        {
            get => Profile.LastName;
            set
            {
                if (value == LastName) return;
                Profile.LastName = value;
                NotifyOfPropertyChange(nameof(LastName));
                NotifyOfPropertyChange(nameof(FormattedName));
                NotifyOfPropertyChange(nameof(ProfileName));
            }
        }

        public string BirthDate
        {
            get
            {
                var b = Profile.BirthDate;
                return b?.ToString("D");
            }
        }

        public DateTime ActualBirthDate
        {
            get => Profile.BirthDate ?? new DateTime(1970, 1, 1);
            set
            {
                if (ActualBirthDate == value) return;
                Profile.BirthDate = value;
                NotifyOfPropertyChange(nameof(ActualBirthDate));
            }
        }

        public string Address
        {
            get => Profile.Address;
            set
            {
                if (Address == value) return;
                Profile.Address = value;
                NotifyOfPropertyChange(nameof(Address));
            }
        }

        public string Note
        {
            get => Profile.Note;
            set
            {
                if (Note == value) return;
                Profile.Note = value;
                NotifyOfPropertyChange(nameof(Note));
            }
        }

        public string InsuranceNumber
        {
            get => CanHaveInsuranceNumber ? Profile.InsuranceNumber : null;
            set
            {
                if (InsuranceNumber == value) return;
                Profile.InsuranceNumber = value;
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
                Profile.PhoneNumbers.Add(num);
                PhoneNumbers.Add(new PhoneNumberViewModel(num,
                    FormattedName, async phone =>
                    {
                        var dia = IoC.Get<IUserDialogs>();
                        if (await dia.ConfirmAsync($"Really remove '{phone.Number}'?", "Confirm delete", "Yes", "No"))
                        {
                            PhoneNumbers.Remove(phone);
                            Profile.PhoneNumbers.Remove(phone.Phone);
                            NotifyOfPropertyChange(nameof(PhoneNumbers));
                        }
                    }));
                NotifyOfPropertyChange(nameof(PhoneNumbers));
            });
            var nav = IoC.Get<INavigationService>();
            Device.BeginInvokeOnMainThread(() => nav.NavigateToInstanceAsync(vm));
        }
    }
}