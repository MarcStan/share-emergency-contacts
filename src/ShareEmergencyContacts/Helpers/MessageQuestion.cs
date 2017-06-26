using System;

namespace ShareEmergencyContacts.Helpers
{
    /// <summary>
    /// A message object that displays two buttons and allows the user to chose an answer.
    /// </summary>
    public class MessageQuestion : MessageBase
    {
        /// <summary>
        /// Creates a new message.
        /// </summary>
        /// <param name="title">The title is optional.</param>
        /// <param name="message">The message is required.</param>
        /// <param name="cancel">The text of the cancel action. This is triggered when the user presses back.</param>
        /// <param name="ok">The text for the ok action.</param>
        /// <param name="onCompleted">An optional callback that will return the user choice. True for ok action, false for cancel action.</param>
        private MessageQuestion(string title, string message, string cancel, string ok, Action<bool> onCompleted = null) :
                base(title, message, null)
        {
            Cancel = cancel;
            Ok = ok;
            OnCompleted = onCompleted;
        }

        /// <summary>
        /// Gets the value displayed to the user in the button to cancel the question.
        /// </summary>
        public string Cancel { get; }

        /// <summary>
        /// Gets the value displayed to the user in the only button to accept the question.
        /// </summary>
        public string Ok { get; }

        /// <summary>
        /// Invoked after the message was displayed. Provides the user value (true for ok, false for cancel).
        /// </summary>
        public new Action<bool> OnCompleted;

        /// <summary>
        /// Internally invoked to trigger oncomplete with the return value.
        /// </summary>
        /// <param name="ok"></param>
        public void Completed(bool ok)
        {
            OnCompleted?.Invoke(ok);
            OnCompleted = null;
        }

        public override void Completed()
        {
            throw new NotSupportedException("Call the Completed overload with a parameter for this message type.");
        }
    }
}