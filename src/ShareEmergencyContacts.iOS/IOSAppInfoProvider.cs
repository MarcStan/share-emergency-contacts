using System;
using UIKit;

namespace ShareEmergencyContacts.iOS
{
    public class IOSAppInfoProvider : IAppInfoProvider
    {
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

        public string MobileCenterKey { get; }

        public IOSAppInfoProvider(string mobileCenterKey)
        {
            MobileCenterKey = mobileCenterKey;
        }
    }

}