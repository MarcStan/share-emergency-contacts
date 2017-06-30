using ShareEmergencyContacts.Models.Data;
using System;
using System.Linq;
using System.Text;

namespace ShareEmergencyContacts.Helpers
{
    public class VCard
    {
        private static readonly char[] _escapedCharacters = { ',', '\\', ';', '\r', '\n' };

        public string ToVCard(EmergencyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var sb = new StringBuilder();
            WriteVCardV4(profile, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Writes the vcard v4 format to the stringbuilder.
        /// That way contacts can also be imported by others without the app.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="sb"></param>
        private static void WriteVCardV4(EmergencyProfile profile, StringBuilder sb)
        {
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:4.0");
            // along with begin, version and end "FN" is the only required property in V4
            // luckily it is also the profile name 
            var writeDirect = new Action<string>(s => sb.AppendLine(s));
            WriteEmergencyContact(profile, writeDirect);
            EncodeAndAppendIfSet("NOTE", profile.Note, writeDirect);

            // everything else is custom format, so use "X-" prefix
            int insuranceId = 0;
            // insurance providers get X-INS{id}-
            foreach (var insurance in profile.InsuranceContacts)
            {
                insuranceId++;
                var id = insuranceId;
                WriteEmergencyContact(insurance, s => sb.AppendLine($"X-INS{id}-{s}"));
            }
            int iceId = 0;
            // emergency contacts get X-ICE{id}-
            foreach (var ice in profile.EmergencyContacts)
            {
                iceId++;
                int id = iceId;
                WriteEmergencyContact(ice, s => sb.AppendLine($"X-ICE{id}-{s}"));
            }
            EncodeAndAppendIfSet("X-BLOOD", profile.BloodType, writeDirect);
            EncodeAndAppendIfSet("X-EXPIRES", DateToString(profile.ExpirationDate), writeDirect);
            if (profile.HeightInCm > 0)
                EncodeAndAppendIfSet("X-HEIGHT", profile.HeightInCm.ToString(), writeDirect);
            if (profile.WeightInLbsTimes100 > 0)
                EncodeAndAppendIfSet("X-WEIGHT", profile.WeightInLbsTimes100.ToString(), writeDirect);
            sb.AppendLine("END:VCARD");
        }

        /// <summary>
        /// Writes information of the baseclass <see cref="EmergencyContact"/> to the functor.
        /// It's called once for each line.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="entry"></param>
        private static void WriteEmergencyContact(EmergencyContact contact, Action<string> entry)
        {
            if (contact.ProfileName == null)
                throw new NotSupportedException("Profile name must be set");

            EncodeAndAppendIfSet("FN", contact.ProfileName, entry);

            // all other properties may be null and thus may not be set
            var f = FormatNameIfPossible(contact.FirstName, contact.LastName);
            EncodeAndAppendIfSet("N", f, entry);
            EncodeAndAppendIfSet("ADR", contact.Address, entry);
            foreach (var num in contact.PhoneNumbers)
            {
                // doesn't seem to have predefines types ("mobile", "cell", etc. are used)
                // so just format our enum
                var t = num.Type.ToString().ToUpper();
                EncodeAndAppendIfSet($"TEL;TYPE={t}", $"{num.Number}", entry);
            }
            EncodeAndAppendIfSet("BDAY", DateToString(contact.BirthDate), entry);
        }

        private static string FormatNameIfPossible(string first, string last)
        {
            if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(last))
                return null;

            return $"{first};{last};;;";
        }

        private static string DateToString(DateTime? date)
        {
            if (date.HasValue)
            {
                var b = date.Value;
                return $"{b.Year}{b.Month:00}{b.Day:00}";
            }
            return null;
        }

        /// <summary>
        /// Only does anything when text is not null.
        /// Encodes the text to be valid vcard format and appens it after the identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="text"></param>
        /// <param name="entry"></param>
        private static void EncodeAndAppendIfSet(string identifier, string text, Action<string> entry)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                // all fields are optional, so don't set if user hasn't set it
                return;
            }
            var formatted = Encode(text);
            entry($"{identifier}:{formatted}");
        }

        /// <summary>
        /// Encodes the string in vcard format, escaping all special chars.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Encode(string text)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (_escapedCharacters.Contains(c))
                {
                    // prepend \ for escape chars
                    sb.Append("\\");
                    // special cases
                    switch (c)
                    {
                        case '\r':
                            c = 'r';
                            break;
                        case '\n':
                            c = 'n';
                            break;
                    }
                }
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}