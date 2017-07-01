using Foundation;
using UIKit;

namespace ShareEmergencyContacts.iOS
{
    public class IOSPhoneDialProvider : IPhoneDialProvider
    {
        public void Dial(string number, string name)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl($"tel:{number}"));
        }
    }
}