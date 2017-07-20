using System.Threading.Tasks;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsCheckPermissions : ICheckPermissions
    {
        public bool PermissionIsInManifest(PermissionType perm)
        {
            return true;
        }

        public bool AppHasPermissionGranted(PermissionType perm)
        {
            return true;
        }

        public async Task<PermissionResult> GrantPermissionAsync(PermissionType perm)
        {
            // TODO: I can't seem to find a way to check for capabilities at runtime
            // seems as from the app perspective e.g. "camera" is always accessible but when user manually denied it in Privacy settings
            // the app will receive a camerafeed that is just a white image
            return PermissionResult.Granted;
        }
    }
}