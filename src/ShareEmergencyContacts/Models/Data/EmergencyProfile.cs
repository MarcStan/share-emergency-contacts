using ShareEmergencyContacts.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public string Citizenship { get; set; }

        public string Passport { get; set; }

        public string Allergies { get; set; }

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

        public EmergencyProfile CloneFull()
        {
            var n = CloneList(PhoneNumbers, p => p.Clone());
            var ice = CloneList(EmergencyContacts, e => e.Clone());
            var ins = CloneList(InsuranceContacts, i => i.Clone());
            return new EmergencyProfile
            {
                Allergies = Allergies,
                BloodType = BloodType,
                Citizenship = Citizenship,
                EmergencyContacts = ice,
                ExpirationDate = ExpirationDate,
                HeightInCm = HeightInCm,
                InsuranceContacts = ins,
                Passport = Passport,
                WeightInKg = WeightInKg,
                Relationship = Relationship,
                Email = Email,
                ProfileName = ProfileName ?? "", // cannot be null
                Note = Note,
                Address = Address,
                InsuranceNumber = InsuranceNumber,
                BirthDate = BirthDate,
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumbers = n
            };
        }

        /// <summary>
        /// Returns if the current profile is blank (no entries whatsoever).
        /// </summary>
        /// <returns></returns>
        public bool IsBlank()
        {
            var isContactBlank = new Func<EmergencyContact, bool>(c =>
            {
                if (c.PhoneNumbers.Any())
                    return false;
                // ProfileName is set by system; it is usually empty now so skip it
                if (BirthDate.HasValue)
                    return false;
                return string.IsNullOrWhiteSpace(c.Address) &&
                       string.IsNullOrWhiteSpace(c.Email) &&
                       string.IsNullOrWhiteSpace(c.FirstName) &&
                       string.IsNullOrWhiteSpace(c.LastName) &&
                       string.IsNullOrWhiteSpace(c.Note) &&
                       string.IsNullOrWhiteSpace(c.Relationship) &&
                       string.IsNullOrWhiteSpace(c.InsuranceNumber);
            });
            // contact is blank if itself is blank or any of its (if any) ICE/INS contacts
            // also check for the extra fields on the profile itself (allergies, etc.)
            return isContactBlank(this) && InsuranceContacts.All(i => isContactBlank(i)) &&
                EmergencyContacts.All(e => isContactBlank(e)) &&
                    string.IsNullOrWhiteSpace(Allergies) &&
                   string.IsNullOrWhiteSpace(BloodType) &&
                   string.IsNullOrWhiteSpace(Citizenship) &&
                   string.IsNullOrWhiteSpace(Passport) &&
                   WeightInKg <= 0 &&
                   HeightInCm <= 0;
        }
    }
}