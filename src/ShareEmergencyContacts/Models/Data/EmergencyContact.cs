using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShareEmergencyContacts.Models.Data
{
    [DebuggerDisplay("{" + nameof(ProfileName) + "}")]
    public class EmergencyContact
    {
        private string _profileName;

        /// <summary>
        /// The unique (nick)name of the current profile.
        /// This property may never be null.
        /// </summary>
        public string ProfileName
        {
            get => _profileName;
            set => _profileName = value ?? throw new ArgumentNullException(nameof(ProfileName), "This property may not be null.");
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Address { get; set; }

        /// <summary>
        /// Because we are resusing this model for instancce details as well, this property exists.
        /// Views only ever allow to set it if the user adds insurance details, otherwise it will be null.
        /// </summary>
        public string InsuranceNumber { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// The relationship of the child contact with the parent.
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// Other textfiled that can be set by the user.
        /// </summary>
        public string Note { get; set; }

        public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();

        public EmergencyContact Clone()
        {
            var n = CloneList(PhoneNumbers, p => p.Clone());
            return new EmergencyContact
            {
                FirstName = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                Address = Address,
                InsuranceNumber = InsuranceNumber,
                Email = Email,
                Relationship = Relationship,
                Note = Note,
                PhoneNumbers = n
            };
        }

        protected List<T> CloneList<T>(List<T> list, Func<T, T> clone)
        {
            var clonedList = new List<T>();
            foreach (var e in list)
            {
                clonedList.Add(clone(e));
            }
            return clonedList;
        }
    }
}