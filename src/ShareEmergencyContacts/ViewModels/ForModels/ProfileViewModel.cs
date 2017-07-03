using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
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
        private readonly EmergencyProfile _profile;

        public ProfileViewModel(EmergencyProfile profile, IWorkWithProfiles mv) : base(profile, false, false)
        {
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));

            EmergencyContacts = profile.EmergencyContacts.Select(c => new ContactViewModel(c, false, true)).ToList();
            InsuranceContacts = profile.InsuranceContacts.Select(c => new ContactViewModel(c, true, true)).ToList();

            DeleteCommand = new Command(() => mv.Delete(Actual));
        }

        public ICommand DeleteCommand { get; }

        public string Weight
        {
            get
            {
                if (_profile.WeightInKg <= 0)
                    return null;

                return $"{_profile.WeightInKg} kg";
            }
        }

        public string Height
        {
            get
            {
                if (_profile.HeightInCm <= 0)
                    return null;

                return $"{_profile.HeightInCm / 100:0.00} m";
            }
        }

        public string BloodType => _profile.BloodType;

        public string ExpirationDate
        {
            get
            {
                var d = _profile.ExpirationDate;
                if (!d.HasValue)
                    return null;
                int x = (d.Value - DateTime.Now.Date).Days;

                var specifier = x == 0 ? "" : $" ({x} day{(x == 1 ? "" : "s")})";
                return $"{d.Value:D}{specifier}";
            }
        }
        public List<ContactViewModel> EmergencyContacts { get; }

        public List<ContactViewModel> InsuranceContacts { get; }

        public EmergencyProfile Actual => _profile;
    }
}