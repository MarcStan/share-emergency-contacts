using Windows.UI.ViewManagement;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsAppInfoProvider : IAppInfoProvider
    {
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

        public string MobileCenterKey { get; }

        public WindowsAppInfoProvider(string mobileCenterKey)
        {
            MobileCenterKey = mobileCenterKey;
        }
    }

}