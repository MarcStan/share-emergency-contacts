namespace ShareEmergencyContacts
{
    /// <summary>
    /// Allows opening the number in the phone dialoer.
    /// </summary>
    public interface IPhoneDialProvider
    {
        /// <summary>
        /// Opens the dialer with the provided number.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="name">Optional name to be displayed alongside the to-be-called number.</param>
        void Dial(string number, string name);
    }
}