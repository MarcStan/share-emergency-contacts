using ShareEmergencyContacts.Models.Data;
using System;

namespace ShareEmergencyContacts.Helpers
{
    public class QrCodeHelper
    {
        private static readonly VCard _vCard = new VCard();

        /// <summary>
        /// Converts the provided profile to qr code.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static string ToQrCodeContent(EmergencyProfile profile)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            return _vCard.ToVCard(profile);
        }
    }
}