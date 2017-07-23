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

        public Task<PermissionResult> GrantPermissionAsync(PermissionType perm)
        {
            // seems as from the app perspective e.g. "camera" is always accessible even when user manually denied it in Privacy settings
            // the app will receive e.g. a camerafeed that is just a white image instead of actually acessing the camera
            return Task.Run(() => PermissionResult.Granted);
        }
    }
}