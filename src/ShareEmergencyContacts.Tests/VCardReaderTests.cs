using FluentAssertions;
using NUnit.Framework;
using ShareEmergencyContacts.Helpers;
using System;

namespace ShareEmergencyContacts.Tests
{
    [TestFixture]
    public class VCardReaderTests
    {
        [Test]
        public void TestV2()
        {
            // from https://en.wikipedia.org/wiki/VCard#Example_of_vCard_files
            var text = "BEGIN:VCARD" + Environment.NewLine +
                       "VERSION:2.1" + Environment.NewLine +
                       "N:Gump;Forrest;;Mr." + Environment.NewLine +
                       "END:VCARD";
            var vcard = new VCardHelper();
            new Action(() => vcard.FromVCard(text)).Should().Throw<VCardException>("because format 2.1 is not supported.");
        }

        [Test]
        public void TestV4()
        {
            // from https://en.wikipedia.org/wiki/VCard#Example_of_vCard_files
            var text = "BEGIN:VCARD" + Environment.NewLine +
                       "VERSION:4.0" + Environment.NewLine +
                       "N:Forrest;Gump;;Mr.;" + Environment.NewLine +
                       "FN:Forrest Gump" + Environment.NewLine +
                       "ORG:Bubba Gump Shrimp Co." + Environment.NewLine +
                       "TITLE:Shrimp Man" + Environment.NewLine +
                       "PHOTO;MEDIATYPE=image/gif:http://www.example.com/dir_photos/my_photo.gif" + Environment.NewLine +
                       "TEL;TYPE=work,voice;VALUE=uri:tel:+1-111-555-1212" + Environment.NewLine +
                       "TEL;TYPE=home,voice;VALUE=uri:tel:+1-404-555-1212" + Environment.NewLine +
                       "ADR;TYPE=WORK,PREF:;;100 Waters Edge;Baytown;LA;30314;United States of America" + Environment.NewLine +
                       "LABEL;TYPE=WORK,PREF:100 Waters Edge\\nBaytown\\, LA 30314\\nUnited States of America" + Environment.NewLine +
                       "ADR;TYPE=HOME:;;42 Plantation St.;Baytown;LA;30314;United States of America" + Environment.NewLine +
                       "LABEL;TYPE=HOME:42 Plantation St.\\nBaytown\\, LA 30314\\nUnited States of America" + Environment.NewLine +
                       "EMAIL:forrestgump@example.com" + Environment.NewLine +
                       "REV:20080424T195243Z" + Environment.NewLine +
                       "x-qq:21588891" + Environment.NewLine +
                       "END:VCARD";

            var vcard = new VCardHelper();
            var profile = vcard.FromVCard(text);
            profile.BloodType.Should().BeNull();
            profile.HeightInCm.Should().Be(-1);
            profile.WeightInKg.Should().Be(-1);
            profile.ProfileName.Should().Be("Forrest Gump");
            profile.FirstName.Should().Be("Forrest");
            profile.LastName.Should().Be("Gump");
            profile.Address.Should().Be("TYPE=WORK,PREF:;;100 Waters Edge;Baytown;LA;30314;United States of America");
        }
    }
}