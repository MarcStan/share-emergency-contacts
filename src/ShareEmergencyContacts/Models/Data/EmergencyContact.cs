using Newtonsoft.Json;
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
        [JsonProperty("p")]
        public string ProfileName
        {
            get => _profileName;
            set => _profileName = value ?? throw new ArgumentNullException(nameof(ProfileName), "This property may not be null.");
        }

        [JsonProperty("fn")]
        public string FirstName { get; set; }

        [JsonProperty("ln")]
        public string LastName { get; set; }

        [JsonProperty("bday")]
        public DateTime? BirthDate { get; set; }

        [JsonProperty("a")]
        public string Address { get; set; }

        [JsonProperty("phones")]
        public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
    }
}