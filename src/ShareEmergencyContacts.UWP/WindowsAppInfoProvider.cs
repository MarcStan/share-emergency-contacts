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

        /// <summary>
        /// Or not. Fucking Microsoft provides no way to check whether the system is running in light or dark mode.
        /// Really.
        /// It's not like the feature was availablr in the previous versions of Windowsphone, way to fuck your own shit up again microsoft..
        /// Just return false because fuck userexperience, right?
        /// </summary>
        public bool SystemThemeIsDark => false;

        public WindowsAppInfoProvider(string mobileCenterKey)
        {
            MobileCenterKey = mobileCenterKey;
        }
    }

}