using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShareEmergencyContacts.Models.Data
{
    [DebuggerDisplay("{" + nameof(FullName) + "}")]
    public class EmergencyContact
    {
        /// <summary>
        /// The unique (nick)name of the current profile.
        /// </summary>
        public string ProfileName { get; set; }

        public string FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public List<PhoneNumber> PhoneNumbers { get; set; }
    }
}