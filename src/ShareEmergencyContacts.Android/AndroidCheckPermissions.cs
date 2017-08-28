using Android.App;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidCheckPermissions : ICheckPermissions
    {
        private readonly Activity _context;
        private readonly Dictionary<int, TaskCompletionSource<PermissionResult>> _permissionTokens;
        private int _nextFreeCode;

        public AndroidCheckPermissions(Activity context)
        {
            _context = context;
            _permissionTokens = new Dictionary<int, TaskCompletionSource<PermissionResult>>();
        }

        public bool PermissionIsInManifest(PermissionType perm)
        {
            var permission = Resolve(perm);
            try
            {
                var info = _context.PackageManager.GetPackageInfo(_context.PackageName, PackageInfoFlags.Permissions);
                return info.RequestedPermissions.Contains(permission);
            }
            catch
            {
                return false;
            }
        }

        private string Resolve(PermissionType perm)
        {
            switch (perm)
            {
                case PermissionType.Camera:
                    return Android.Manifest.Permission.Camera;
                case PermissionType.Storage:
                    return Android.Manifest.Permission.WriteExternalStorage;
                default:
                    throw new ArgumentOutOfRangeException(nameof(perm), perm, null);
            }
        }

        public bool AppHasPermissionGranted(PermissionType perm)
        {
            if (!PermissionIsInManifest(perm))
                throw new NotSupportedException("Permission must be added to the manifest before it can be granted.");

            var permission = Resolve(perm);
            return ContextCompat.CheckSelfPermission(_context, permission) == Permission.Granted;
        }

        public async Task<PermissionResult> GrantPermissionAsync(PermissionType perm)
        {
            if (AppHasPermissionGranted(perm))
                return PermissionResult.Granted;

            // request permission
            var permission = Resolve(perm);

            // retarded android bullshit: the result is not returned here but in the main activity
            // so we proxied it to the
            // use unique id to PermissionRequestAnswered which will need a unique token per request
            var id = _nextFreeCode;
            _nextFreeCode++;
            // finally store a task that completes once the answer is received
            var tcs = new TaskCompletionSource<PermissionResult>();
            _permissionTokens.Add(id, tcs);

            // run permission request by android (may or may not spawn dialog)
            ActivityCompat.RequestPermissions(_context, new[] { permission }, id);
            // await the result proxied to PermissionRequestAnswered
            await tcs.Task;
            return tcs.Task.Result;
        }

        public void PermissionRequestAnswered(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (!_permissionTokens.ContainsKey(requestCode))
                throw new NotSupportedException("Missing token for request code");


            var token = _permissionTokens[requestCode];
            _permissionTokens.Remove(requestCode);

            if (grantResults.Length != 1)
                throw new NotSupportedException("Only one permission at a time is supported!");

            var granted = grantResults[0] == Permission.Granted;
            if (!granted)
            {
                // returns false when the user checked "never ask again" as per https://stackoverflow.com/a/34612503
                if (!ActivityCompat.ShouldShowRequestPermissionRationale(_context, permissions[0]))
                {
                    token.SetResult(PermissionResult.AlwaysDenied);
                }
                else
                {
                    // first time user denied he cannot check "always deny"
                    token.SetResult(PermissionResult.Denied);
                }

            }
            else
            {
                // set answer so that GrantPermission can keep executing
                token.SetResult(PermissionResult.Granted);
            }
        }
    }
}