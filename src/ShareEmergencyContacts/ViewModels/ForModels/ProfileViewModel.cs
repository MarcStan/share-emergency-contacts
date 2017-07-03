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
        public ProfileViewModel(EmergencyProfile profile, Action<EmergencyProfile> delete) : base(profile, false, false, null)
        {
            Actual = profile ?? throw new ArgumentNullException(nameof(profile));

            EmergencyContacts = new BindableCollection<ContactViewModel>(profile.EmergencyContacts.Select(c => new ContactViewModel(c, false, true, p => EmergencyContacts.Remove(p))));
            InsuranceContacts = new BindableCollection<ContactViewModel>(profile.InsuranceContacts.Select(c => new ContactViewModel(c, true, true, p => InsuranceContacts.Remove(p))));

            DeleteCommand = new Command(() => delete?.Invoke(Actual));
        }

        public ICommand DeleteCommand { get; }

        public override bool IsEditable
        {
            get => base.IsEditable;
            set
            {
                if (IsEditable == value) return;
                base.IsEditable = value;
                foreach (var i in InsuranceContacts)
                {
                    i.IsEditable = IsEditable;
                }
                foreach (var e in EmergencyContacts)
                {
                    e.IsEditable = IsEditable;
                }
            }
        }

        public string Weight
        {
            get
            {
                if (Actual.WeightInKg <= 0)
                    return null;

                return $"{Actual.WeightInKg} kg";
            }
        }

        public string Height
        {
            get
            {
                if (Actual.HeightInCm <= 0)
                    return null;

                return $"{Actual.HeightInCm / 100:0.00} m";
            }
        }

        public string BloodType => Actual.BloodType;

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
        public BindableCollection<ContactViewModel> EmergencyContacts { get; }

        public BindableCollection<ContactViewModel> InsuranceContacts { get; }

        public EmergencyProfile Actual { get; }
    }
}