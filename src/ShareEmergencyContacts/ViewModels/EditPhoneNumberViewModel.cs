using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class EditPhoneNumberViewModel : Screen
    {
        private readonly Action<PhoneNumber> _onSave;
        private string _number;
        private PhoneType _selectedPhoneType;
        private BindableCollection<PhoneType> _phoneTypes;

        public EditPhoneNumberViewModel(PhoneNumber phone, Action<PhoneNumber> onSave)
        {
            Analytics.TrackEvent(AnalyticsEvents.EditPhoneNumber);
            _onSave = onSave;
            var values = Enum.GetValues(typeof(PhoneType)).Cast<PhoneType>();
            PhoneTypes = new BindableCollection<PhoneType>(values);

            SelectedPhoneType = phone?.Type ?? PhoneType.Mobile;
            Number = phone?.Number;
            SaveCommand = new Command(Save);
        }

        public ICommand SaveCommand { get; }

        public string Number
        {
            get => _number;
            set
            {
                if (value == _number) return;
                _number = value;
                NotifyOfPropertyChange(nameof(Number));
            }
        }

        public PhoneType SelectedPhoneType
        {
            get => _selectedPhoneType;
            set
            {
                if (value == _selectedPhoneType) return;
                _selectedPhoneType = value;
                NotifyOfPropertyChange(nameof(SelectedPhoneType));
            }
        }

        public BindableCollection<PhoneType> PhoneTypes
        {
            get => _phoneTypes;
            set
            {
                if (Equals(value, _phoneTypes)) return;
                _phoneTypes = value;
                NotifyOfPropertyChange(nameof(PhoneTypes));
            }
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Number))
            {
                IoC.Get<IUserDialogs>().Alert("No phone number provided.");
                return;
            }
            Analytics.TrackEvent(AnalyticsEvents.SavePhoneNumber);
            _onSave(new PhoneNumber(SelectedPhoneType, Number));
            Device.BeginInvokeOnMainThread(() => IoC.Get<INavigationService>().GoBackAsync());
        }
    }
}