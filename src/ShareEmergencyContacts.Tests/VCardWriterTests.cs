using FluentAssertions;
using NUnit.Framework;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;

namespace ShareEmergencyContacts.Tests
{
    [TestFixture]
    public class VCardWriterTests
    {
        [Test]
        public void EmptyProfileShouldThrow()
        {
            var vcard = new VCardHelper();
            var empty = new EmergencyProfile();
            new Action(() => empty.ProfileName = null).ShouldThrow<ArgumentNullException>("because null is not allowed on this property.");
            new Action(() => vcard.ToVCard(empty)).ShouldThrow<NotSupportedException>("because the one property name is not set");
        }

        [Test]
        public void NullProfileShouldThrow()
        {
            var vcard = new VCardHelper();

            new Action(() => vcard.ToVCard(null)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ProfileWithNameShouldNotThrow()
        {
            var vcard = new VCardHelper();

            var empty = new EmergencyProfile();
            empty.ProfileName = "Name 1";
            var result = vcard.ToVCard(empty);
            result.Should().Contain("Name 1");
        }

        [Test]
        public void WriteFullProfile()
        {
            var vcard = new VCardHelper();

            var full = new EmergencyProfile
            {
                ProfileName = "Marc",
                FirstName = "Marc",
                LastName = "Stan",
                Address = "Street 1" + Environment.NewLine +
                        "12345 LegitCity",
                BirthDate = new DateTime(1989, 1, 13),
                HeightInCm = 200,
                WeightInKg = 100,
                Note = "This is a note",
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber(PhoneType.Home, "555 12345"),
                    new PhoneNumber(PhoneType.Mobile, "+1 555 12345"),
                },
                ExpirationDate = new DateTime(2037, 2, 13),
                InsuranceContacts = new List<EmergencyContact>
                {
                    new EmergencyContact
                    {
                        ProfileName = "INSURANCE",
                        InsuranceNumber = "#1234",
                        PhoneNumbers = new List<PhoneNumber>
                        {
                            new PhoneNumber(PhoneType.Work, "+41 1414")
                        }
                    },
                    new EmergencyContact
                    {
                        ProfileName = "POLICE",
                        InsuranceNumber = "#1234.68.123",
                        PhoneNumbers = new List<PhoneNumber>
                        {
                            new PhoneNumber(PhoneType.Work, "+49 110")
                        }
                    }
                },
                EmergencyContacts = new List<EmergencyContact>
                {
                    new EmergencyContact
                    {
                        ProfileName = "Dad",
                        PhoneNumbers = new List<PhoneNumber>
                        {
                            new PhoneNumber(PhoneType.Work, "1234567890")
                        }
                    },
                    new EmergencyContact
                    {
                        ProfileName = "Mom",
                        PhoneNumbers = new List<PhoneNumber>
                        {
                            new PhoneNumber(PhoneType.Work, "1234567890 2")
                        }
                    }
                }
            };
            var result = vcard.ToVCard(full);
            var lines = result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int index = 0;
            lines[index++].Should().Be("BEGIN:VCARD");
            lines[index++].Should().Be("VERSION:4.0");
            lines[index++].Should().Be("FN:Marc");
            lines[index++].Should().Be("N:Marc;Stan;;;");
            lines[index++].Should().Be("NOTE:This is a note");
            lines[index++].Should().Be("ADR:Street 1\\n12345 LegitCity");
            lines[index++].Should().Be("TEL;TYPE=HOME:555 12345");
            lines[index++].Should().Be("TEL;TYPE=MOBILE:+1 555 12345");
            lines[index++].Should().Be("BDAY:19890113");
            lines[index++].Should().Be("X-INS-1-FN:INSURANCE");
            lines[index++].Should().Be("X-INS-1-X-INSNUM:#1234");
            lines[index++].Should().Be("X-INS-1-TEL;TYPE=WORK:+41 1414");
            lines[index++].Should().Be("X-INS-2-FN:POLICE");
            lines[index++].Should().Be("X-INS-2-X-INSNUM:#1234.68.123");
            lines[index++].Should().Be("X-INS-2-TEL;TYPE=WORK:+49 110");
            lines[index++].Should().Be("X-ICE-1-FN:Dad");
            lines[index++].Should().Be("X-ICE-1-TEL;TYPE=WORK:1234567890");
            lines[index++].Should().Be("X-ICE-2-FN:Mom");
            lines[index++].Should().Be("X-ICE-2-TEL;TYPE=WORK:1234567890 2");
            lines[index++].Should().Be("X-EXPIRES:20370213");
            lines[index++].Should().Be("X-HEIGHT:200");
            lines[index++].Should().Be("X-WEIGHT:100");
            lines[index++].Should().Be("END:VCARD");
        }

        [Test]
        public void WritePartialProfile()
        {
            var vcard = new VCardHelper();

            var full = new EmergencyProfile
            {
                ProfileName = "Marc",
                FirstName = "Marc",
                LastName = "Stan",
                Address = "Street 1" + Environment.NewLine +
                          "12345 LegitCity",
                BirthDate = new DateTime(1989, 1, 13),
                Note = "This is a note",
                PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber(PhoneType.Home, "555 12345"),
                    new PhoneNumber(PhoneType.Mobile, "+1 555 12345"),
                },
                EmergencyContacts = new List<EmergencyContact>
                {
                    new EmergencyContact
                    {
                        ProfileName = "Dad",
                        PhoneNumbers = new List<PhoneNumber>
                        {
                            new PhoneNumber(PhoneType.Work, "1234567890")
                        }
                    }
                }
            };
            var result = vcard.ToVCard(full);
            var lines = result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int index = 0;
            lines[index++].Should().Be("BEGIN:VCARD");
            lines[index++].Should().Be("VERSION:4.0");
            lines[index++].Should().Be("FN:Marc");
            lines[index++].Should().Be("N:Marc;Stan;;;");
            lines[index++].Should().Be("NOTE:This is a note");
            lines[index++].Should().Be("ADR:Street 1\\n12345 LegitCity");
            lines[index++].Should().Be("TEL;TYPE=HOME:555 12345");
            lines[index++].Should().Be("TEL;TYPE=MOBILE:+1 555 12345");
            lines[index++].Should().Be("BDAY:19890113");
            lines[index++].Should().Be("X-ICE-1-FN:Dad");
            lines[index++].Should().Be("X-ICE-1-TEL;TYPE=WORK:1234567890");
            lines[index++].Should().Be("END:VCARD");
        }
    }
}
