using System;
using System.Linq;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidAppInfoProvider : IAppInfoProvider
    {
        public string UserFriendlyVersion
        {
            get
            {
                var context = Forms.Context;
                return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            }
        }

        public Version Version
        {
            get
            {
                var v = UserFriendlyVersion;
                Version version;
                // assume that the user friendly version is a valid version first
                if (Version.TryParse(v, out version))
                    return version;

                // otherwise assume it to be in format "1.0.0-beta" where version is upfront, followed by some text
                // not perfect parsing (e.g. would allow "1...00", ".1.", ".." etc) but the TryParse will fail then and we error anyway
                var versionPrefix = new string(v.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray());
                if (Version.TryParse(versionPrefix, out version))
                    return version;

                throw new NotSupportedException("On Android the exact version is parsed from the UserFriendlyVersion (AndroidManifest.xml -> android:versionName). " +
                                                "UserFriendlyVersion must thus be valid version (e.g. 1.0.0) or valid version followed by some random text (e.g. v1.0.0-beta2)." +
                                                $"The found value '{v}' did not match either of those two formats.");
            }
        }
    }

}