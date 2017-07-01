using UIKit;

namespace ShareEmergencyContacts.iOS
{
    public class IOSClipboardProvider : IClipboardProvider
    {
        public void Copy(string text)
        {
            var clipboard = UIPasteboard.General;
            clipboard.String = text;
        }
    }
}