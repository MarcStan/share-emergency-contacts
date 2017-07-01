using Acr.UserDialogs;
using Caliburn.Micro;
using ShareEmergencyContacts.Models.Data;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels.ForModels
{
    /// <summary>
    ///  Wrapper to dislay data for <see cref="PhoneNumber"/>.
    /// </summary>
    public class PhoneNumberViewModel
    {
        private readonly PhoneNumber _phone;
        private readonly string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="name">A name to be displayed alongside the number when calling. Necessary on some platforms.</param>
        public PhoneNumberViewModel(PhoneNumber phone, string name)
        {
            _phone = phone;
            _name = name;
            DialNumber = new Command(CallAsync);
            CopyNumber = new Command(Copy);
        }

        public string Type => _phone.Type.ToString();

        public string Number => _phone.Number;

        public ICommand DialNumber { get; }

        public ICommand CopyNumber { get; }

        public async void CallAsync()
        {
            var dial = IoC.Get<IPhoneDialProvider>();
            try
            {
                dial.Dial(Number, _name);
            }
            catch
            {
                var dia = IoC.Get<IUserDialogs>();
                var r = await dia.ConfirmAsync("Failed to dial number. Do you want to copy it to the clipboard?", "Invalid number", "Yes", "No");
                if (r)
                {
                    Copy();
                }
            }
        }

        private void Copy()
        {
            var clip = IoC.Get<IClipboardProvider>();
            clip.Copy(Number);
            var dia = IoC.Get<IUserDialogs>();
            dia.Toast($"Copied {Number} to clipboard.").Dispose();
        }
    }
}