using AVFoundation;
using Foundation;
using System;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.iOS
{
    public class IOSCheckPermissions : ICheckPermissions
    {
        public bool PermissionIsInManifest(PermissionType perm)
        {
            try
            {
                var v = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString(ResolveManifestName(perm)));
                return v != null;
            }
            catch
            {
                return false;
            }
        }

        public bool AppHasPermissionGranted(PermissionType perm)
        {
            return GetAuthStatus(perm) == AVAuthorizationStatus.Authorized;
        }

        private NSString ResolveRuntimeName(PermissionType perm)
        {
            switch (perm)
            {
                case PermissionType.Camera:
                    return AVMediaType.Video;
                default:
                    throw new ArgumentOutOfRangeException(nameof(perm), perm, null);
            }
        }

        public async Task<PermissionResult> GrantPermissionAsync(PermissionType perm)
        {
            var auth = GetAuthStatus(perm);
            if (auth == AVAuthorizationStatus.Authorized)
                return PermissionResult.Granted;

            var permission = ResolveRuntimeName(perm);
            var completionTask = new TaskCompletionSource<PermissionResult>();
            AVCaptureDevice.RequestAccessForMediaType(permission, granted =>
            {
                var result = PermissionResult.Granted;
                if (!granted)
                {
                    // first time access for any permission returns "not determined"
                    result = auth == AVAuthorizationStatus.NotDetermined
                        ? PermissionResult.Denied
                        : PermissionResult.AlwaysDenied;
                }
                completionTask.SetResult(result);
            });
            await completionTask.Task;
            return completionTask.Task.Result;
        }

        private string ResolveManifestName(PermissionType perm)
        {
            switch (perm)
            {
                case PermissionType.Camera:
                    return "NSCameraUsageDescription";
                case PermissionType.Storage:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(perm), perm, null);
            }
        }

        private AVAuthorizationStatus GetAuthStatus(PermissionType perm)
        {
            // technically we could skip this check if we compiled for iOS < 10
            if (!PermissionIsInManifest(perm))
                throw new NotSupportedException("As of iOS 10 permissions must be added to the info.plist");

            var permission = ResolveRuntimeName(perm);
            var authStatus = AVCaptureDevice.GetAuthorizationStatus(permission);
            return authStatus;
        }
    }
}