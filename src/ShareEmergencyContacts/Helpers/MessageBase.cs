using System;

namespace ShareEmergencyContacts.Helpers
{
    /// <summary>
    /// The base object for all messages.
    /// </summary>
    public class MessageBase
    {
        protected MessageBase(string title, string message, Action onCompleted)
        {
            Title = title;
            Message = message;
            OnCompleted = onCompleted;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; }

        /// <summary>
        /// Invoked after the message was displayed to the user.
        /// </summary>
        public Action OnCompleted;

        /// <summary>
        /// Called by the system when the message was displayed to the user and he dismissed it.
        /// Will invoke the event and dispose of it.
        /// </summary>
        public virtual void Completed()
        {
            OnCompleted?.Invoke();
            OnCompleted = null;
        }
    }
}