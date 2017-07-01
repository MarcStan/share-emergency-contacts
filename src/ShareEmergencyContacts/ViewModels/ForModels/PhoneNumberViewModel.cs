using ShareEmergencyContacts.Models.Data;

namespace ShareEmergencyContacts.ViewModels.ForModels
{
    /// <summary>
    ///  Wrapper to dislay data for <see cref="PhoneNumber"/>.
    /// </summary>
    public class PhoneNumberViewModel
    {
        private readonly PhoneNumber _phone;

        public PhoneNumberViewModel(PhoneNumber phone)
        {
            _phone = phone;
        }

        public string Type => _phone.Type.ToString();

        public string Number => _phone.Number;
    }
}