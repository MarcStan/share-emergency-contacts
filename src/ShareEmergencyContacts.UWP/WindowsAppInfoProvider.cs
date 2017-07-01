using System;
using Windows.ApplicationModel;
using Windows.UI.ViewManagement;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsAppInfoProvider : IAppInfoProvider
    {
        public string UserFriendlyVersion => Version.ToString(3);

        public Version Version
        {
            get
            {
                var v = Package.Current.Id.Version;
                // revision cannot be set via UI (only by editing by hand)
                return new Version(v.Major, v.Minor, v.Build, v.Revision);
            }
        }

        public int ScreenWidth
        {
            get
            {
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                //var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                return (int)bounds.Width;
                //var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
            }
        }

        public int ScreenHeight
        {
            get
            {
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                //var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                return (int)bounds.Height;
            }
        }
    }

}