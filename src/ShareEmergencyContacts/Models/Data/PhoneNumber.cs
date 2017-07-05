using System;

namespace ShareEmergencyContacts.Models.Data
{
    /// <summary>
    /// Stores information about a phone number.
    /// </summary>
    public class PhoneNumber
    {
        public PhoneType Type { get; set; }

        public string Number { get; set; }

        public PhoneNumber(PhoneType type, string number)
        {
            if (string.IsNullOrEmpty(number))
                throw new ArgumentNullException(nameof(number));

            Type = type;
            Number = number;
        }

        public override string ToString()
        {
            return $"{Type}: {Number}";
        }
    }
}