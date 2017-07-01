using ShareEmergencyContacts.Models.Data;
using System.Collections.Generic;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// Xamarin XAML does not support bindings to child properties "{Binding SelectedProfile, Path=FirstName}" so
    /// this class has to act as a wrapper for all entries.
    /// </summary>
    public class SelectedProfileViewModel : ViewModelBase
    {
        private EmergencyProfile _selectedProfile;

        public string ProfileName => SelectedProfile.ProfileName;

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SelectedProfile.FirstName))
                    return string.IsNullOrWhiteSpace(SelectedProfile.LastName) ? null : SelectedProfile.LastName;
                if (string.IsNullOrWhiteSpace(SelectedProfile.LastName))
                    return string.IsNullOrWhiteSpace(SelectedProfile.FirstName) ? null : SelectedProfile.FirstName;

                return $"{SelectedProfile.FirstName} {SelectedProfile.LastName}";
            }
        }

        public string BirthDate
        {
            get
            {
                var b = SelectedProfile.BirthDate;
                return b?.ToString("D");
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

        public string ExpirationDate
        {
            get
            {
                var d = SelectedProfile.ExpirationDate;
                return d?.ToString("D");
            }
        }
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