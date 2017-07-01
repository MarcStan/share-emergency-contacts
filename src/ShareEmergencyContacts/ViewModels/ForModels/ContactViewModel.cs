using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels.ForModels
{
    /// <summary>
    /// Wrapper for the <see cref="EmergencyContact"/> to display all its data.
    /// </summary>
    public class ContactViewModel
    {
        private readonly bool _displayInsuranceNumber;
        private readonly bool _isChild;
        private readonly EmergencyContact _profile;

        public ContactViewModel(EmergencyContact profile, bool displayInsuranceNumber, bool isChild)
        {
            _displayInsuranceNumber = displayInsuranceNumber;
            _isChild = isChild;
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            PhoneNumbers = profile.PhoneNumbers.Select(p => new PhoneNumberViewModel(p, Name)).ToList();
        }

        public string ProfileName => _profile.ProfileName;

        public StackOrientation NameOrientation => _isChild ? StackOrientation.Horizontal : StackOrientation.Vertical;

        public int NameSize => _isChild ? 24 : 16;

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

        public string InsuranceNumber => _displayInsuranceNumber ? _profile.InsuranceNumber : null;

        public List<PhoneNumberViewModel> PhoneNumbers { get; }
    }
}