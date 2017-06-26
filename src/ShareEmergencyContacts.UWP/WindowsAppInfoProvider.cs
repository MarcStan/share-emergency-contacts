using System;
using Windows.ApplicationModel;

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
    }

}