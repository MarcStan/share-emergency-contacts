namespace ShareEmergencyContacts.Helpers
{
    public static class DataLimits
    {
        /// <summary>
        /// Maximum number of phone numbers per contact.
        /// This is per ICE/INS as well as the profile itself. (Making foor (<see cref="MaxSubContacts"/> + 1) * <see cref="MaxPhoneNumbers"/> total numbers allowed.
        /// This limit is arbitrary.
        /// </summary>
        public const int MaxPhoneNumbers = 4;

        /// <summary>
        /// The maximum number of allowed sub contacts per profile.
        /// This limit is arbitrary.
        /// </summary>
        public const int MaxSubContacts = 6;

        /// <summary>
        /// The total number of characters allowed per profile.
        /// This limit is required because qr code does not support unlimited barcodes.
        /// The more characters are used, the bigger the qr code will get (and thus the smaller the squares will be).
        /// Upper limit of qr code is 4096 if using only A-Z0-9 and a few special chars (note: NO LOWERCASE). See: https://en.wikipedia.org/wiki/QR_code#Storage
        /// This limit also requires use of errorcorrection L (Low) which we already use.
        /// By adding lowercase and more special chars this limit is reduced further.
        /// Manual experiments have shown that the ZXing barcode generator will crash when using more than ~2500 characters.
        /// To be on the safe side set the limit way lower.
        /// </summary>
        public const int MaxCharacterCount = 2048;
    }
}