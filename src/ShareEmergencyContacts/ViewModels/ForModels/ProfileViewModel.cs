using Caliburn.Micro;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels.ForModels
{
    /// <summary>
    /// Wrapper for the <see cref="EmergencyProfile"/> to display all its data.
    /// </summary>
    public class ProfileViewModel : ContactViewModel
    {
        private readonly Action<ProfileViewModel> _delete;
        private readonly Action<ProfileViewModel> _rename;
        private BindableCollection<ContactViewModel> _insuranceContacts;
        private BindableCollection<ContactViewModel> _emergencyContacts;
        private bool _weightIsValid;
        private bool _heightIsValid;
        private EmergencyProfile _actual;

        public ProfileViewModel(EmergencyProfile profile, Action<ProfileViewModel> delete, Action<ProfileViewModel> rename) : base(profile, false, false, null)
        {
            _delete = delete;
            _rename = rename;
            Actual = profile ?? throw new ArgumentNullException(nameof(profile));
            EmergencyContacts = new BindableCollection<ContactViewModel>(profile.EmergencyContacts.Select(c => new ContactViewModel(c, false, true,
                p =>
                {
                    EmergencyContacts.Remove(p);
                    profile.EmergencyContacts.Remove(p.Profile);
                    NotifyOfPropertyChange(nameof(EmergencyContacts));
                })));
            InsuranceContacts = new BindableCollection<ContactViewModel>(profile.InsuranceContacts.Select(c => new ContactViewModel(c, true, true,
                p =>
                {
                    InsuranceContacts.Remove(p);
                    profile.InsuranceContacts.Remove(p.Profile);
                    NotifyOfPropertyChange(nameof(InsuranceContacts));
                })));

            DeleteCommand = new Command(() => delete?.Invoke(this));
            RenameCommand = new Command(() => rename?.Invoke(this));
            SendEmailCommand = new Command(() =>
            {
                Device.OpenUri(new Uri($"mailto:{Email}"));
            });
        }

        public ProfileViewModel Clone()
        {
            var clone = Actual.CloneFull();
            return new ProfileViewModel(clone, _delete, _rename);
        }

        public ICommand DeleteCommand { get; }

        public ICommand RenameCommand { get; }

        public ICommand SendEmailCommand { get; }

        public string Weight
        {
            get
            {
                if (Actual.WeightInKg <= 0)
                    return null;

                return $"{Actual.WeightInKg} kg";
            }
        }

        public string Allergies
        {
            get => Actual.Allergies;
            set
            {
                if (value == Allergies) return;
                Actual.Allergies = value;
                NotifyOfPropertyChange(nameof(Allergies));
            }
        }

        public string Citizenship
        {
            get => Actual.Citizenship;
            set
            {
                if (value == Citizenship) return;
                Actual.Citizenship = value;
                NotifyOfPropertyChange(nameof(Citizenship));
            }
        }

        public string Passport
        {
            get => Actual.Passport;
            set
            {
                if (value == Passport) return;
                Actual.Passport = value;
                NotifyOfPropertyChange(nameof(Passport));
            }
        }

        public string ActualWeightInKg
        {
            get => Actual.WeightInKg <= 0 ? null : Actual.WeightInKg.ToString();
            set
            {
                if (ActualWeightInKg == value) return;
                WeightIsValid = int.TryParse(value, out int x);
                if (WeightIsValid)
                    Actual.WeightInKg = x;
                else if (string.IsNullOrWhiteSpace(value))
                    Actual.WeightInKg = -1;

                NotifyOfPropertyChange(nameof(ActualWeightInKg));
            }
        }

        public bool WeightIsValid
        {
            get => _weightIsValid;
            set
            {
                if (value == _weightIsValid) return;
                _weightIsValid = value;
                NotifyOfPropertyChange(nameof(WeightIsValid));
            }
        }

        public string Height
        {
            get
            {
                if (Actual.HeightInCm <= 0)
                    return null;

                return $"{Actual.HeightInCm / 100.0:0.00} m";
            }
        }

        public string ActualHeightInCm
        {
            get => Actual.HeightInCm <= 0 ? "" : Actual.HeightInCm.ToString();
            set
            {
                if (ActualHeightInCm == value) return;
                HeightIsValid = int.TryParse(value, out int x);
                if (HeightIsValid)
                    Actual.HeightInCm = x;
                else if (string.IsNullOrWhiteSpace(value))
                    Actual.HeightInCm = -1;

                NotifyOfPropertyChange(nameof(ActualHeightInCm));
            }
        }

        public bool HeightIsValid
        {
            get => _heightIsValid;
            set
            {
                if (value == _heightIsValid) return;
                _heightIsValid = value;
                NotifyOfPropertyChange(nameof(HeightIsValid));
            }
        }

        public string BloodType
        {
            get => Actual.BloodType;
            set
            {
                if (BloodType == value) return;
                Actual.BloodType = value;
                NotifyOfPropertyChange(nameof(BloodType));
            }
        }

        public string ExpirationDate
        {
            get
            {
                var d = Actual.ExpirationDate;
                if (!d.HasValue)
                    return null;
                int x = (d.Value - DateTime.Now.Date).Days;

                var specifier = x == 0 ? "" : $" ({x} day{(x == 1 ? "" : "s")})";
                return $"{d.Value:D}{specifier}";
            }
        }

        public BindableCollection<ContactViewModel> EmergencyContacts
        {
            get => _emergencyContacts;
            set
            {
                if (Equals(value, _emergencyContacts)) return;
                _emergencyContacts = value;
                NotifyOfPropertyChange(nameof(EmergencyContacts));
            }
        }

        public BindableCollection<ContactViewModel> InsuranceContacts
        {
            get => _insuranceContacts;
            set
            {
                if (Equals(value, _insuranceContacts)) return;
                _insuranceContacts = value;
                NotifyOfPropertyChange(nameof(InsuranceContacts));
            }
        }

        public EmergencyProfile Actual
        {
            get => _actual;
            set
            {
                if (Equals(value, _actual)) return;
                _actual = value;
                Profile = value;
                NotifyOfPropertyChange(nameof(Actual));
            }
        }
    }
}