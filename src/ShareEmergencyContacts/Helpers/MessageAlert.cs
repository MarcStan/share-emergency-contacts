using System;

namespace ShareEmergencyContacts.Helpers
{
    /// <summary>
    /// A message object that displays text and can be dismissed.
    /// </summary>
    public class MessageAlert : MessageBase
    {
        /// <summary>
        /// Creates a new message.
        /// </summary>
        /// <param name="title">The title is optional.</param>
        /// <param name="message">The message is required.</param>
        /// <param name="cancel">The value to be displayed in the only button (ok/cancel/yes/no, etc.)</param>
        /// <param name="onCompleted">An optional callback that is executed when the message is dismissed by the user.</param>
        private MessageAlert(string title, string message, string cancel, Action onCompleted = null) :
            base(title, message, onCompleted)
        {
            Cancel = cancel;
        }

        /// <summary>
        /// Gets the value displayed to the user in the only button.
        /// </summary>
        public string Cancel { get; }
    }
}