using System.Threading.Tasks;

namespace ShareEmergencyContacts
{
    public interface ICheckPermissions
    {
        /// <summary>
        /// Checks whether the permission is infact in the manifest.
        /// Without this the permission can never be granted.
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        bool PermissionIsInManifest(PermissionType perm);

        /// <summary>
        /// Checks if the app already has the permission.
        /// This only works if the permission is specified in the manifest.
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        bool AppHasPermissionGranted(PermissionType perm);

        /// <summary>
        /// Prompts the user to grant the permission at runtime and returns true if he does.
        /// Note that if the user selected "never grant" this method will simply return false as if he just denied the permission.
        /// If the user previously granted the permission it will simply return true.
        /// </summary>
        /// <param name="perm"></param>
        /// <exception cref="System.NotSupportedException">Thrown when <see cref="PermissionIsInManifest"/> returns false for the specific permission.</exception>
        /// <returns></returns>
        Task<PermissionResult> GrantPermissionAsync(PermissionType perm);
    }

    public enum PermissionResult
    {
        /// <summary>
        /// Indicates that the permission was granted.
        /// Used on all platforms.
        /// </summary>
        Granted,
        /// <summary>
        /// Indicates that the permission was denied by the user.
        /// On android this is the case when the user presses "denied" on the grant permission prompt. The prompt may be displayed to the user again.
        /// For iOS this is returned the first time (as the dialog is only prompting the user once).
        /// On UWP this is never returned as UWP always prentends to grant access (if user has it disabled in settings e.g. the camerafeed will be white).
        /// </summary>
        Denied,
        /// <summary>
        /// Indicates that the permission is globally denied.
        /// On android this is the case when the user checks the "never ask again" checkbox. Every future call will not display a prompt to the user but instead return this instantly.
        /// On iOS this is the case when the user selected "deny" the first time and is always returned for any future check (first check returned Denied).
        /// On UWP this is never returned as UWP always prentends to grant access (if user has it disabled in settings e.g. the camerafeed will be white).
        /// </summary>
        AlwaysDenied
    }

    public enum PermissionType
    {
        Camera
    }
}