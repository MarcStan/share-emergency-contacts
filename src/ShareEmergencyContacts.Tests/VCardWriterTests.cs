using FluentAssertions;
using NUnit.Framework;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using System;

namespace ShareEmergencyContacts.Tests
{
    [TestFixture]
    public class VCardWriterTests
    {
        [Test]
        public void EmptyProfileShouldThrow()
        {
            var vcard = new VCard();
            var empty = new EmergencyProfile();
            new Action(() => empty.ProfileName = null).ShouldThrow<ArgumentNullException>("because null is not allowed on this property.");
            new Action(() => vcard.ToVCard(empty)).ShouldThrow<NotSupportedException>("because the one property name is not set");
        }

        [Test]
        public void NullProfileShouldThrow()
        {
            var vcard = new VCard();

            new Action(() => vcard.ToVCard(null)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ProfileWithNameShouldNotThrow()
        {
            var vcard = new VCard();

            var empty = new EmergencyProfile();
            empty.ProfileName = "Name 1";
            var result = vcard.ToVCard(empty);
            result.Should().Contain("Name1");
        }
    }
}
