using ShareEmergencyContacts.Models.Data;
using System.Collections.Generic;

namespace ShareEmergencyContacts.ViewModels
{
    public class SelectedProfileViewModel : ViewModelBase
    {
        private EmergencyProfile _selectedProfile;

        public string ProfileName => SelectedProfile.ProfileName;

        public string FirstName => SelectedProfile.FirstName;

        public string LastName => SelectedProfile.LastName;

        public string BirthDate
        {
            get
            {
                var b = SelectedProfile.BirthDate;
                return b?.ToString("d");
            }
        }

        public string Address => SelectedProfile.Address;

        public string Note => SelectedProfile.Note;

        public List<PhoneNumber> PhoneNumbers => SelectedProfile.PhoneNumbers;

        public string Weight
        {
            get
            {
                if (SelectedProfile.WeightInKg <= 0)
                    return null;

                return $"{SelectedProfile.WeightInKg} kg";
            }
        }

        public string Height
        {
            get
            {
                if (SelectedProfile.HeightInCm <= 0)
                    return null;

                return $"{SelectedProfile.HeightInCm / 100:0.00} m";
            }
        }

        public string BloodType => SelectedProfile.BloodType;

        public List<EmergencyContact> EmergencyContacts => SelectedProfile.EmergencyContacts;

        public List<EmergencyContact> InsuranceContacts => SelectedProfile.InsuranceContacts;

        public EmergencyProfile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (Equals(value, _selectedProfile)) return;
                _selectedProfile = value;
                NotifyOfPropertyChange(nameof(SelectedProfile));
            }
        }
    }
}