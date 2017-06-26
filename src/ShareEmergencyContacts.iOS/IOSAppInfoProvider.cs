using ShareEmergencyContacts;
using Foundation;
using System;

namespace ShareEmergencyContacts.iOS
{
    public class IOSAppInfoProvider : IAppInfoProvider
    {
        readonly NSString _version;
        readonly NSString _userVersion;

        public IOSAppInfoProvider()
        {
            // this is actually checked by apple when uploading
            _version = new NSString("CFBundleVersion");
            // localizable version
            _userVersion = new NSString("CFBundleShortVersionString");
        }

        public string UserFriendlyVersion
        {
            get
            {
                var v = NSBundle.MainBundle.InfoDictionary.ValueForKey(_userVersion);
                return v.ToString();
            }
        }

        public Version Version
        {
            get
            {
                var v = NSBundle.MainBundle.InfoDictionary.ValueForKey(_version);
                // we expect it to be a valid version
                return Version.Parse(v.ToString());
            }
        }
    }

}