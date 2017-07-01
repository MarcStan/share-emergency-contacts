using System;

namespace ShareEmergencyContacts.Models.Data
{
    /// <summary>
    /// Stores information about a phone number.
    /// </summary>
    public struct PhoneNumber : IEquatable<PhoneNumber>
    {
        public PhoneType Type { get; }

        public string Number { get; }

        public PhoneNumber(PhoneType type, string number)
        {
            if (string.IsNullOrEmpty(number))
                throw new ArgumentNullException(nameof(number));

            Type = type;
            Number = number;
        }

        public bool Equals(PhoneNumber other)
        {
            return Type == other.Type && string.Equals(Number, other.Number);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PhoneNumber && Equals((PhoneNumber)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Type * 397) ^ (Number != null ? Number.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{Type}: {Number}";
        }

        public static bool operator ==(PhoneNumber lhs, PhoneNumber rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(PhoneNumber lhs, PhoneNumber rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}