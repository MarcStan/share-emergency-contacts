using System;
using System.Collections.Generic;

namespace ShareEmergencyContacts.Models.Data
{
    /// <summary>
    /// The user profile is more detailed than the emergency contact
    /// because the user is the one at risk.
    /// </summary>
    public class MyProfile : EmergencyContact
    {
        /// <summary>
        /// Stores the weight value in lbs * 100.
        /// That way aftercomma values are not truncated.
        /// </summary>
        public int WeightInLbsTimes100 { get; set; }

        /// <summary>
        /// The height in cm.
        /// </summary>
        public int HeightInCm { get; set; }

        /// <summary>
        /// Blood type of the person, duh.
        /// </summary>
        public string BloodType { get; set; }

        /// <summary>
        /// The datetime of expiration.
        /// Only used on received profiles and allows auto. delete.
        /// Null if no expiration.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Dictionary of custom fields if any set.
        /// Never null but may be empty.
        /// </summary>
        public Dictionary<string, string> Other { get; set; }

        /// <summary>
        /// List of emergency contacts.
        /// Never null but may be empty.
        /// </summary>
        public List<EmergencyContact> EmergencyContacts { get; set; }

        /// <summary>
        /// List of insurance providers.
        /// Never null but may be empty.
        /// </summary>
        public List<EmergencyContact> InsuranceContacts { get; set; }
    }
}