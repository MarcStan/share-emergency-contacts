using FluentAssertions;
using NUnit.Framework;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;

namespace ShareEmergencyContacts.Tests
{
    [TestFixture]
    public class VCardReadWriteTests
    {
        [Test]
        public void SaveAndLoadFullProfile()
        {
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

            var vcard = new VCardHelper();
            var text = vcard.ToVCard(full);

            var contact2 = vcard.FromVCard(text);

            contact2.ShouldBeEquivalentTo(full, options => options.IncludingNestedObjects());

            // reverse test
            contact2.EmergencyContacts[0].ProfileName = "definitely not correct";
            new Action(() => contact2.ShouldBeEquivalentTo(full, o => o.IncludingNestedObjects())).ShouldThrow<Exception>("because we just changed the name");
        }
    }
}