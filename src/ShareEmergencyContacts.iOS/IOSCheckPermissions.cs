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
            // technically we could skip this check if we compiled for iOS < 10
            if (!PermissionIsInManifest(perm))
                throw new NotSupportedException("As of iOS 10 permissions must be added to the info.plist");

            var permission = ResolveRuntimeName(perm);
            var authStatus = AVCaptureDevice.GetAuthorizationStatus(permission);
            return authStatus == AVAuthorizationStatus.Authorized;
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
            if (AppHasPermissionGranted(perm))
                return PermissionResult.Granted;

            var permission = ResolveRuntimeName(perm);
            var completionTask = new TaskCompletionSource<PermissionResult>();
            AVCaptureDevice.RequestAccessForMediaType(permission, granted =>
            {
                completionTask.SetResult(granted ? PermissionResult.Granted : PermissionResult.Denied);
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(perm), perm, null);
            }
        }
    }
}