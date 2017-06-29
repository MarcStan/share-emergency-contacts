using Newtonsoft.Json;
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
        [JsonProperty("p")]
        public string ProfileName { get; set; }

        [JsonProperty("n")]
        public string FullName { get; set; }

        [JsonProperty("bday")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("a")]
        public string Address { get; set; }

        [JsonProperty("phones")]
        public List<PhoneNumber> PhoneNumbers { get; set; }
    }
}