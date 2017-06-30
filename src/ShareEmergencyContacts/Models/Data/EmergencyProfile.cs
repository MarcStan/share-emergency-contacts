using ShareEmergencyContacts.Helpers;
using System;
using System.Collections.Generic;

namespace ShareEmergencyContacts.Models.Data
{
    /// <summary>
    /// The user profile is more detailed than the emergency contact
    /// because the user is the one at risk.
    /// </summary>
    public class EmergencyProfile : EmergencyContact
    {
        private static readonly VCardHelper _vCardHelper = new VCardHelper();

        /// <summary>
        /// Stores the weight value in kg.
        /// That way aftercomma values are not truncated.
        /// </summary>
        public int WeightInKg { get; set; }

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
        /// List of emergency contacts.
        /// Never null but may be empty.
        /// </summary>
        public List<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

        /// <summary>
        /// List of insurance providers.
        /// Never null but may be empty.
        /// </summary>
        public List<EmergencyContact> InsuranceContacts { get; set; } = new List<EmergencyContact>();

        /// <summary>
        /// Reads the provided textfile and returns the content or null if unable to parse.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static EmergencyProfile ParseFromText(string content)
        {
            try
            {
                return _vCardHelper.FromVCard(content);
            }
            catch (VCardException)
            {
                // TODO: log
                return null;
            }
        }

        /// <summary>
        /// Converts the profile to text.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static string ToRawText(EmergencyProfile profile)
        {
            return _vCardHelper.ToVCard(profile);
        }
    }
}