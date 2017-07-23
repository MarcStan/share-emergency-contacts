using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareEmergencyContacts.Helpers
{
    /// <summary>
    /// Helper class to read VCARDS.
    /// It supports several custom extensions such as:
    /// X-INS-%int%- Insurance contact details (%int% being an id to allow multiple contacts to be listed).
    /// X-ICE-%int%- ICE (In Case of Emergency contacts) (%int% being an id to allow multiple contacts to be listed).
    /// X-INSNUM - insurance number (only used for X-INS-%int%- contacts).
    /// X-HEIGHT - height in cm of the user (int; -1 or 0 if not set)
    /// X-WEIGHT - weight in kg of the user (int; -1 or 0 if not set)
    /// X-BLOODTYPE - blood group of the user (string; null if not set)
    /// X-EXPIRES - date of expiry (DateTime or null) time component is ignored, always expires the day after
    /// </summary>
    public class VCardHelper
    {
        private static readonly char[] _escapedCharacters = { ',', '\\', ';', '\n' };

        public static class VDataKey
        {
            public static class Required
            {
                public const string FullName = "FN";
            }

            public const string Name = "N";
            public const string Email = "EMAIL";
            public const string Note = "NOTE";
            public const string Address = "ADR";
            public const string PhoneNumber = "TEL";
            public const string Birthday = "BDAY";

            public static class Ext
            {
                public const string InsuranceNumber = "X-INSNUM";
                public const string Relationship = "X-RELATIONSHIP";
                public const string BloodType = "X-BLOODTYPE";
                public const string Expires = "X-EXPIRES";
                public const string Height = "X-HEIGHT";
                public const string Weight = "X-WEIGHT";
                public const string Allergies = "X-ALLERGIES";
                public const string Citizenship = "X-CITIZENSHIP";
                public const string Passport = "X-PASSPORT";
            }
        }

        /// <summary>
        /// Converts the provided profile to its string representation in the VCard format.
        /// Currently using the VCard version 4.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public string ToVCard(EmergencyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            var sb = new StringBuilder();
            WriteVCardV4(profile, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Reads the vcard from the provided text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public EmergencyProfile FromVCard(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new VCardException("Input string is null.", null);

            // sender should always use \n only. to be save just remove all \r as well
            text = text.Replace("\r", "");

            // multiplatform, so split all linebreaks
            return FromVCard(text.Split(new[] { "\n" }, StringSplitOptions.None));
        }

        /// <summary>
        /// Reads the vcard from the provided lines.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public EmergencyProfile FromVCard(string[] lines)
        {
            if (lines == null || lines.Length < 3)
            {
                throw new VCardException("Not enough lines to be a vcard!", lines);
            }
            // at a minimum we need BEGIN, END and VERSION (empty vcard)
            var processedLines = lines.ToList();
            if (processedLines[0] != "BEGIN:VCARD")
            {
                throw new VCardException("First line must be BEGIN:VCARD", lines);
            }
            if (processedLines[processedLines.Count - 1] != "END:VCARD")
            {
                throw new VCardException("Last line must be END:VCARD", lines);
            }
            // remove the lines
            processedLines.RemoveAt(0);
            processedLines.RemoveAt(processedLines.Count - 1);

            var v = GetValue(processedLines, "VERSION");
            if (v != "4.0")
                throw new VCardException("Only v4 is currently supported", lines);

            // now we parse the actual data
            try
            {
                return ParseVCardV4(processedLines);
            }
            catch (Exception e)
            {
                throw new VCardException("Failed to parse VCARD data.", lines, e);
            }
        }

        /// <summary>
        /// Assumes BEGIN, END and VERSION to be checked and removed from the provided set.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private EmergencyProfile ParseVCardV4(List<string> lines)
        {
            var p = new EmergencyProfile();
            // read base data
            ReadEmergencyContact(p, lines);

            // read "X-" data
            p.BloodType = GetValue(lines, VDataKey.Ext.BloodType);
            p.ExpirationDate = GetDateValue(lines, VDataKey.Ext.Expires);
            p.HeightInCm = GetIntValue(lines, VDataKey.Ext.Height);
            p.WeightInKg = GetIntValue(lines, VDataKey.Ext.Weight);
            p.Allergies = GetValue(lines, VDataKey.Ext.Allergies);
            p.Citizenship = GetValue(lines, VDataKey.Ext.Citizenship);
            p.Passport = GetValue(lines, VDataKey.Ext.Passport);

            // read emergency and insurance contacts
            var iceLines = lines.Where(l => l.StartsWith("X-ICE-")).ToList();
            var insuranceLines = lines.Where(l => l.StartsWith("X-INS-")).ToList();
            lines.RemoveAll(l => iceLines.Contains(l));
            lines.RemoveAll(l => insuranceLines.Contains(l));
            // filter each into segments

            var ice = ReadListedContacts(iceLines, "X-ICE-");
            var insurance = ReadListedContacts(insuranceLines, "X-INS-");

            // dictionary now contains valid contact for each
            foreach (var e in ice)
            {
                var newLines = e.Value;
                var contact = new EmergencyContact();
                ReadEmergencyContact(contact, newLines);
                p.EmergencyContacts.Add(contact);
            }
            foreach (var i in insurance)
            {
                var c = new EmergencyContact();
                ReadEmergencyContact(c, i.Value);
                p.InsuranceContacts.Add(c);
            }

            return p;
        }

        /// <summary>
        /// Reads normal contact info from the provided lines but expects each line to be prefixed.
        /// E.g. normal line "FN:name" is expected to be "prefix-int-FN:name"
        /// The resulting contacts are then added to the dictionary where each unique id contains info about one contact.
        /// This allows storing a list of contacts inside a VCARD.
        /// </summary>
        /// <example>
        /// X-ICE-1-FN:dad
        /// X-ICE-1-TEL:TYPE=MOBILE:1234
        /// X-ICE-2-FN:mom
        /// X-ICE-2-TEL:TYPE=MOBILE:4321
        /// 
        /// By calling this function with prefix "X-ICE-" it is stripped, the following int is parsed and creates the dicitonary buckets..
        /// And the dictionary lines then contain
        /// [key 1:
        /// FN:dad
        /// TEL:TYPE=MOBILE:1234
        /// ]
        /// [key 2:
        /// FN:mom
        /// TEL:TYPE=MOBILE:4321]
        /// </example>
        /// <param name="lines"></param>
        /// <param name="prefix">A prefix to be stripped. Note that this function expects an integer to be directly after the prefix and the integer must be delimited with another '-' at the end.</param>
        /// <returns></returns>
        private static Dictionary<int, List<string>> ReadListedContacts(List<string> lines, string prefix)
        {
            var ice = new Dictionary<int, List<string>>();
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var id = line.Substring(prefix.Length);
                var idx = id.IndexOf('-');
                if (idx == -1)
                {
                    throw new FormatException($"'{prefix.Length}' prefixed line was not followed up with '-' after id. Expected '{prefix}int-' but found: " + line);
                }
                id = id.Substring(0, idx);
                if (!int.TryParse(id, out int num))
                    throw new FormatException($"Id in '{prefix}' line was not an int: " + line);

                if (!ice.ContainsKey(num))
                    ice.Add(num, new List<string>());

                // prefix is prefix.Length + idLength + "-"
                var fullPrefixLength = prefix.Length + id.Length + 1;
                ice[num].Add(line.Substring(fullPrefixLength));
            }
            return ice;
        }

        /// <summary>
        /// Writes contact info from the provided lines to the existing contact instance.
        /// This allows reuse for both <see cref="EmergencyContact"/> and <see cref="EmergencyProfile"/>.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="lines"></param>
        private void ReadEmergencyContact(EmergencyContact contact, List<string> lines)
        {
            // only required value, ensure it exists
            var name = GetValue(lines, VDataKey.Required.FullName);
            if (string.IsNullOrWhiteSpace(name))
                throw new FormatException("VCARD missing required property FN.");
            contact.ProfileName = name;

            // all other values are optional; set to null if not found
            var format = GetValue(lines, VDataKey.Name);

            if (format != null && format.Contains(";"))
            {
                var splits = format.Split(new[] { ';' }, StringSplitOptions.None);
                contact.FirstName = splits[0];
                contact.LastName = splits[1];
                if ((!string.IsNullOrWhiteSpace(contact.FirstName) || !string.IsNullOrWhiteSpace(contact.LastName)) &&
                    !(contact is EmergencyProfile))
                {
                    // insurance and emergency contacts don't need the name if their first/last is set
                    contact.ProfileName = "";
                }
            }
            contact.Email = GetValue(lines, VDataKey.Email);
            contact.Address = GetValue(lines, VDataKey.Address);
            contact.Note = GetValue(lines, VDataKey.Note);
            contact.InsuranceNumber = GetValue(lines, VDataKey.Ext.InsuranceNumber);
            contact.Relationship = GetValue(lines, VDataKey.Ext.Relationship);
            string tel;
            do
            {
                tel = GetValue(lines, VDataKey.PhoneNumber);
                if (tel != null && tel.Contains(":"))
                {
                    // we get TYPE=Foo:value back
                    var split = tel.Split(':');
                    var type = split[0];
                    // type is stil "TYPE=FOO", so split
                    // some phones however may store VCARDS as TEL:FOO:value so we'd get "FOO:VALUE" -> just use full "FOO" if it doesn't contain "="
                    if (type.Contains("="))
                        type = type.Split('=')[1];
                    var num = split[1];
                    PhoneType t;
                    if (!Enum.TryParse(type, true, out t))
                        t = PhoneType.Other;

                    var n = new PhoneNumber(t, num);
                    contact.PhoneNumbers.Add(n);
                }
            } while (tel != null);

            contact.BirthDate = GetDateValue(lines, "BDAY");
        }

        /// <summary>
        /// Returns the int value or -1 if not found.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private int GetIntValue(List<string> lines, string identifier)
        {
            var s = GetValue(lines, identifier);
            if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int x))
                return x;

            return -1;
        }

        /// <summary>
        /// Returns the date or null if not parsable or not found.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private DateTime? GetDateValue(List<string> lines, string identifier)
        {
            var bday = GetValue(lines, identifier);
            if (!string.IsNullOrWhiteSpace(bday) &&
                bday.Length == 8 &&
                int.TryParse(bday.Substring(0, 4), out int y) &&
                int.TryParse(bday.Substring(4, 2), out int m) &&
                int.TryParse(bday.Substring(6, 2), out int d))
                return new DateTime(y, m, d);
            return null;
        }

        /// <summary>
        /// Returns the value of the specific identifer and removes the line from the list if found.
        /// </summary>
        /// <param name="processedLines"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private static string GetValue(List<string> processedLines, string identifier)
        {
            var usedIdentifier = identifier + ":";
            string line = processedLines.FirstOrDefault(l => l.StartsWith(usedIdentifier));
            if (line == null)
            {
                // possible that it is something with a prefix, e.g. "TEL;TYPE=FOO:<actual>"
                usedIdentifier = identifier + ";";
                line = processedLines.FirstOrDefault(l => l.StartsWith(usedIdentifier));
                if (line == null)
                {
                    // nope, not found
                    return null;
                }
            }
            line = Decode(line);

            processedLines.Remove(line);
            return line.Substring(usedIdentifier.Length);
        }


        /// <summary>
        /// Writes the vcard v4 format to the stringbuilder.
        /// That way contacts can also be imported by others without the app.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="sb"></param>
        private static void WriteVCardV4(EmergencyProfile profile, StringBuilder sb)
        {
            sb.Append("BEGIN:VCARD\n");
            sb.Append("VERSION:4.0\n");
            // along with begin, version and end "FN" is the only required property in V4
            // luckily it is also the profile name 
            var writeDirect = new Action<string>(s => sb.Append(s + "\n"));
            WriteEmergencyContact(profile, writeDirect);

            // everything else is custom format, so use "X-" prefix
            int insuranceId = 0;
            // insurance providers get X-INS-{id}-
            foreach (var insurance in profile.InsuranceContacts)
            {
                insuranceId++;
                var id = insuranceId;
                WriteEmergencyContact(insurance, s => sb.Append($"X-INS-{id}-{s}\n"));
            }
            int iceId = 0;
            // emergency contacts get X-ICE-{id}-
            foreach (var ice in profile.EmergencyContacts)
            {
                iceId++;
                int id = iceId;
                WriteEmergencyContact(ice, s => sb.Append($"X-ICE-{id}-{s}\n"));
            }
            EncodeAndAppendIfSet(VDataKey.Ext.BloodType, profile.BloodType, writeDirect);
            EncodeAndAppendIfSet(VDataKey.Ext.Expires, DateToString(profile.ExpirationDate), writeDirect);
            if (profile.HeightInCm > 0)
                EncodeAndAppendIfSet(VDataKey.Ext.Height, profile.HeightInCm.ToString(), writeDirect);
            if (profile.WeightInKg > 0)
                EncodeAndAppendIfSet(VDataKey.Ext.Weight, profile.WeightInKg.ToString(), writeDirect);
            EncodeAndAppendIfSet(VDataKey.Ext.Allergies, profile.Allergies, writeDirect);
            EncodeAndAppendIfSet(VDataKey.Ext.Citizenship, profile.Citizenship, writeDirect);
            EncodeAndAppendIfSet(VDataKey.Ext.Passport, profile.Passport, writeDirect);
            sb.Append("END:VCARD");
        }

        /// <summary>
        /// Writes information of the baseclass <see cref="EmergencyContact"/> to the functor.
        /// It's called once for each line.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="entry"></param>
        private static void WriteEmergencyContact(EmergencyContact contact, Action<string> entry)
        {
            var fn = contact.ProfileName;
            if (contact.ProfileName == null)
                fn = contact.FirstName + " " + contact.LastName;
            if (fn == null || fn == " ")
                throw new NotSupportedException("Profile name must be set");

            EncodeAndAppendIfSet(VDataKey.Required.FullName, fn, entry);

            // all other properties may be null and thus may not be set
            EncodeAndAppendIfSet(VDataKey.Name, FormatNameIfPossible(contact.FirstName, contact.LastName), entry, false);
            EncodeAndAppendIfSet(VDataKey.Email, contact.Email, entry);
            EncodeAndAppendIfSet(VDataKey.Note, contact.Note, entry);
            EncodeAndAppendIfSet(VDataKey.Ext.InsuranceNumber, contact.InsuranceNumber, entry);
            EncodeAndAppendIfSet(VDataKey.Ext.Relationship, contact.Relationship, entry);
            EncodeAndAppendIfSet(VDataKey.Address, contact.Address, entry);
            foreach (var num in contact.PhoneNumbers)
            {
                // doesn't seem to have predefines types ("mobile", "cell", etc. are used)
                // so just format our enum
                var t = num.Type.ToString().ToUpper();
                EncodeAndAppendIfSet($"{VDataKey.PhoneNumber};TYPE={t}", $"{num.Number}", entry);
            }
            EncodeAndAppendIfSet(VDataKey.Birthday, DateToString(contact.BirthDate), entry);
        }

        private static string FormatNameIfPossible(string first, string last)
        {
            if (string.IsNullOrWhiteSpace(first) && string.IsNullOrWhiteSpace(last))
                return null;

            return $"{Encode(first)};{Encode(last)};;;";
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
        /// <param name="encode">Defaults to true. Will encode the provided text. If it is already encoded in a VCARD compatible way, set to false.</param>
        private static void EncodeAndAppendIfSet(string identifier, string text, Action<string> entry, bool encode = true)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                // all fields are optional, so don't set if user hasn't set it
                return;
            }
            var formatted = encode ? Encode(text) : text;
            entry($"{identifier}:{formatted}");
        }

        /// <summary>
        /// Encodes the string in vcard format, escaping all special chars.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Encode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            var sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '\r')
                    continue;

                if (_escapedCharacters.Contains(c))
                {
                    // prepend \ for escape chars
                    sb.Append("\\");
                    // special cases
                    switch (c)
                    {
                        case '\n':
                            c = 'n';
                            break;
                    }
                }
                sb.Append(c);
            }
            // remove \r, \n and \r\n and force it to be only \n inline
            return sb.ToString();
        }

        /// <summary>
        /// Decodes the string from vcard format, removing all the escaping of chars.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Decode(string text)
        {
            var sb = new StringBuilder();
            bool insertEscaped = false;
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '\\' && i < text.Length - 1)
                {
                    var next = text[i + 1];
                    if (_escapedCharacters.Contains(next) || next == 'n' || next == 'r')
                    {
                        // current char is \, next char is one of the escaped
                        // -> skip current
                        insertEscaped = true;
                        continue;
                    }
                }
                if (insertEscaped)
                {
                    // special cases
                    switch (c)
                    {
                        case 'r':
                            c = '\r';
                            break;
                        case 'n':
                            c = '\n';
                            break;
                    }
                    insertEscaped = false;
                }
                sb.Append(c);
            }
            return sb.Replace("\r", "").Replace("\n", Environment.NewLine).ToString();
        }
    }
}