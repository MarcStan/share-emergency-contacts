using System;
using Windows.ApplicationModel.Calls;
using Windows.Foundation.Metadata;
using Windows.System;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsPhoneDialProvider : IPhoneDialProvider
    {
        public void Dial(string number, string name)
        {
            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.Calls.CallsPhoneContract", 1))
            {
                // on phone this prompts with a textbox to edit the number and then calls it on "call"
                PhoneCallManager.ShowPhoneCallUI(number, name ?? "call");
            }
            else
            {
                // this prompts to install an app that can handle tel: on desktop because PhoneCallManager is not supported
                // on phone it would display "add contact" with the details but shouldn't because the other part of the if should be called on a phone
                Launcher.LaunchUriAsync(new Uri($"tel:{number}", UriKind.Absolute));
            }
        }
    }
}