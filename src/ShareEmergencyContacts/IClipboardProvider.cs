namespace ShareEmergencyContacts
{
    public interface IClipboardProvider
    {
        /// <summary>
        /// Copies the provided text to clipboard.
        /// </summary>
        /// <param name="text"></param>
        void Copy(string text);
    }
}