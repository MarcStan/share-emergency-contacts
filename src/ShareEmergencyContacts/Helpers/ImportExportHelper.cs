using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareEmergencyContacts.Helpers
{
    public class ImportExportHelper
    {
        /// <summary>
        /// Formats all contacts into a single file.
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="profiles"></param>
        /// <returns></returns>
        public static string ToFile(IList<EmergencyProfile> contacts, IList<EmergencyProfile> profiles)
        {
            var buffer = new StringBuilder();
            if (contacts.Any())
            {
                buffer.AppendLine("# received contacts");

                foreach (var c in contacts)
                {
                    WriteContact(c, buffer);
                }
                if (profiles.Any())
                    buffer.AppendLine();
            }

            if (profiles.Any())
            {
                buffer.AppendLine("# my profiles");
                foreach (var p in profiles)
                {
                    WriteContact(p, buffer);
                }
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Reads a file for import
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="contacts"></param>
        /// <param name="profiles"></param>
        public static bool FromFile(string[] lines, out IList<EmergencyProfile> contacts, out IList<EmergencyProfile> profiles)
        {
            bool isProfiles = false;
            var list = new List<string>();
            contacts = new List<EmergencyProfile>();
            profiles = new List<EmergencyProfile>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line))
                    continue;
                if (line.StartsWith("#"))
                {
                    if (line == "# received contacts")
                        isProfiles = false;
                    if (line == "# my profiles")
                        isProfiles = true;

                    list.Clear();
                    continue;
                }
                // seperator between entries
                if (line == "---")
                {
                    var contact = EmergencyProfile.ParseFromText(string.Join(Environment.NewLine, list));
                    if (contact == null)
                        return false;
                    if (isProfiles)
                        profiles.Add(contact);
                    else
                        contacts.Add(contact);

                    list.Clear();
                    continue;
                }
                list.Add(line);
            }
            if (list.Any())
            {
                // last entry
                var contact = EmergencyProfile.ParseFromText(string.Join(Environment.NewLine, list));
                if (contact == null)
                    return false;
                if (isProfiles)
                    profiles.Add(contact);
                else
                    contacts.Add(contact);

                list.Clear();
            }
            return true;
        }

        private static void WriteContact(EmergencyProfile p, StringBuilder sb)
        {
            var vcard = EmergencyProfile.ToRawText(p);
            var split = vcard.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var s in split)
            {
                sb.AppendLine(s);
            }
            sb.AppendLine("---");
            sb.AppendLine();
        }
    }
}