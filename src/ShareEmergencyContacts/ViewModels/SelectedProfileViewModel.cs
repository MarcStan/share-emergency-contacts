using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// Viewmodel for a single selected profile with all its emergency contacts and 
    /// </summary>
    public class SelectedProfileViewModel : ViewModelBase
    {
        private ProfileViewModel _selected;

        public SelectedProfileViewModel(EmergencyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            Selected = new ProfileViewModel(profile);
        }

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
    }
}