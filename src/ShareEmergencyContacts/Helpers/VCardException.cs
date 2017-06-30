using System;

namespace ShareEmergencyContacts.Helpers
{
    public class VCardException : FormatException
    {
        public VCardException(string message, string[] lines, Exception e) : base(message + Environment.NewLine + string.Join(Environment.NewLine, lines), e)
        {

        }

        public VCardException(string message, string[] lines) : base(message + Environment.NewLine + string.Join(Environment.NewLine, lines))
        {

        }
    }
}