using Foundation;
using System;
using UIKit;

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

        public int ScreenWidth
        {
            get
            {
                int width;
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    // MainScreen.CurrentMode will reflect zoom mode on iOS8+
                    var bounds = UIScreen.MainScreen.NativeBounds;
                    width = (int)bounds.Width;
                }
                else
                {
                    //All older devices are portrait by design so treat the default bounds as such
                    var bounds = UIScreen.MainScreen.Bounds;
                    width = Math.Min((int)bounds.Width, (int)bounds.Height);
                }
                return width;
            }
        }

        public int ScreenHeight
        {
            get
            {
                int height;
                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    // MainScreen.CurrentMode will reflect zoom mode on iOS8+
                    var bounds = UIScreen.MainScreen.NativeBounds;
                    height = (int)bounds.Height;
                }
                else
                {
                    //All older devices are portrait by design so treat the default bounds as such
                    var bounds = UIScreen.MainScreen.Bounds;
                    height = Math.Max((int)bounds.Width, (int)bounds.Height);
                }
                return height;
            }
        }
    }

}