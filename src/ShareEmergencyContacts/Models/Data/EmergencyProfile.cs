using Newtonsoft.Json;
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
        /// <summary>
        /// Stores the weight value in lbs * 100.
        /// That way aftercomma values are not truncated.
        /// </summary>
        [JsonProperty("w")]
        public int WeightInLbsTimes100 { get; set; }

        /// <summary>
        /// The height in cm.
        /// </summary>
        [JsonProperty("h")]
        public int HeightInCm { get; set; }

        /// <summary>
        /// Blood type of the person, duh.
        /// </summary>
        [JsonProperty("b")]
        public string BloodType { get; set; }

        /// <summary>
        /// The datetime of expiration.
        /// Only used on received profiles and allows auto. delete.
        /// Null if no expiration.
        /// </summary>
        [JsonProperty("exp")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Dictionary of custom fields if any set.
        /// Never null but may be empty.
        /// </summary>
        [JsonProperty("o")]
        public Dictionary<string, string> Other { get; set; }

        /// <summary>
        /// List of emergency contacts.
        /// Never null but may be empty.
        /// </summary>
        [JsonProperty("e")]
        public List<EmergencyContact> EmergencyContacts { get; set; }

        /// <summary>
        /// List of insurance providers.
        /// Never null but may be empty.
        /// </summary>
        [JsonProperty("i")]
        public List<EmergencyContact> InsuranceContacts { get; set; }

        /// <summary>
        /// Reads the provided textfile and returns the content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static EmergencyProfile ParseFromText(string content)
        {
            var obj = JsonConvert.DeserializeObject<EmergencyProfile>(content);
            return obj;
        }

        /// <summary>
        /// Converts the profile to text.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static string ToRawText(EmergencyProfile profile)
        {
            var json = JsonConvert.SerializeObject(profile, Formatting.Indented);
            return json;
        }
    }
}