using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels.ForModels
{
    /// <summary>
    ///  Wrapper to dislay data for <see cref="PhoneNumber"/>.
    /// </summary>
    public class PhoneNumberViewModel : PropertyChangedBase
    {
        private readonly string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="name">A name to be displayed alongside the number when calling. Necessary on some platforms.</param>
        /// <param name="remove"></param>
        public PhoneNumberViewModel(PhoneNumber phone, string name, Action<PhoneNumberViewModel> remove)
        {
            Phone = phone;
            _name = name;
            DialNumber = new Command(CallAsync);
            CopyNumber = new Command(Copy);
            EditNumber = new Command(Edit);
            DeleteNumber = new Command(() => remove(this));
        }

        public PhoneNumber Phone { get; }

        public string Type
        {
            get => Phone.Type.ToString();
            set
            {
                if (Type == value) return;
                if (!Enum.TryParse(value, out PhoneType t))
                    throw new NotSupportedException($"Value '{value}' not part of phone type enum.");
                Phone.Type = t;
                NotifyOfPropertyChange(nameof(Type));
            }
        }

        public string Number
        {
            get => Phone.Number;
            set
            {
                if (Number == value) return;
                Phone.Number = value;
                NotifyOfPropertyChange(nameof(Number));
            }
        }

        public ICommand DialNumber { get; }

        public ICommand CopyNumber { get; }

        public ICommand EditNumber { get; }

        public ICommand DeleteNumber { get; }

        public bool IsEditable { get; set; }

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
            dia.Toast($"Copied {Number} to clipboard.");
        }

        private void Edit()
        {
            var vm = new EditPhoneNumberViewModel(Phone, p =>
            {
                Number = p.Number;
                Type = p.Type.ToString();
            });
            IoC.Get<INavigationService>().NavigateToInstanceAsync(vm);
        }
    }
}