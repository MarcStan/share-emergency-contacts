# VersionPatcher

Xamarin provides no support for this and the versions are scattered throughout lots of files:

AndroidManifest.xml for Android
Info.plist for iOS
Package.appxmanifest for UWP

Additionally all projects have AssemblyInfo.cs with actual compiled version info ([assembly: AssemblyVersion("1.0.0.0")]

This tool can be executed to update all these at once.

# Platform specifics

Each platform has its own ideas on how to handle versioning.

Simply put: Each platform requires that the provided version is larger than the previously uploaded version.

UWP checks Package.appxmanifest Version="" property which can be set via UI (only first 3 values) or manually (all 4 values)
iOS checks CFBundleVersion in Info.plist which is a string and can have any value
Android checks android:versionCode in AndroidManifest.xml which is an int

Additionally iOS offers CFBundleShortVersionString which is the user friendly version (can be any text)
And Android offers android:versionName (can be any text).

# Convention

This applies the same version to all platforms. SemVer is recommened (only major.minor.build) but any value that can be converted to Version is ok.

Given a Version 1.2.3.

UWP Package.appxmanifest Version is set to "1.2.3.0"
iOS CFBundleVersion and CFBundleShortVersionString are both set to "1.2.3"
Android android:versionName is set to "1.2.3" and android:versionCode is set to the user provided value

# Command line

```
v|version - must be valid version(e.g. "1.2.3", "1.0.0.0").
c|csharp - path to the C# file containing "[assembly: AssemblyVersion(", "[assembly: AssemblyFileVersion(" and optionally "[assembly: AssemblyInformationalVersion(" will set all three to the new value with 0 for revision (e.g. "1.2.3.0")
a|android - path to the AndroidManifest.xml will set the versionName to e.g. "1.2.3"
i|ios - path to the Info.plist file, will set both CFBundleShortVersionString and CFBundleVersion to e.g. "1.2.3"
u|uwp - path to the Package.appxmanifest file. Will set the Version to the provided value
appCode - (Requires a|android parameter as well). Will set the android appCode to the new int value (Either provide valid int or the string "++". Providing "++" will increment the existing int my 1). Note that in order to upload android versions, this int must be larger than any previously provided value. Either increment it by 1 for each release, or calculate it from version (e.g. multiply each version by 1000 more than the next smaller one. E.g. "1.2.3" -> "1 * 1000 * 1000 + 2 * 1000 + 3, of course no version may individually be larger than 999 or the App store upload might fail due too smaller version number).
```

Tip for AssemblyInfo.cs: Use a shared assembly info file [like so](https://stackoverflow.com/questions/6771694/shared-assemblyinfo-for-uniform-versioning-across-the-solution).

It only needs to be updated once and each project that includes it will use the new version number on next compile.

Downside: You need to edit each project manually after creation to include the shared file and edit the newly created AssemblyInfo file to not have its own version.

If you don't want to use it, you'll have to call this tool multiple times, once for each path of the individual AssemblyInfo files (e.g. vpatch -c "project\ios\Properties\AssemblyInfo.cs", vpatch -c "project\android\Properties\AssemblyInfo.cs", etc.).

# Usage

Set only C# version:
> Vpatch.exe -v 1.2.3 -c "%projects%\my\app\GlobalAssemblyInfo.cs

Set C#, Android, iOS and UWP version:
> Vpatch.exe -v 1.2.3 -c "%projects%\my\app\GlobalAssemblyInfo.cs -a %projects%\my\app\android\AndroidManifest.xml -i %projects%\my\app\ios\Info.plist -u %projects%\my\app\uwp\Package.appxmanifest

Increment only android AppCode
> Vpatch.exe -a %projects%\my\app\android\AndroidManifest.xml -appCode ++

Note that by using "-appCode ++" the existing int is incremented by 1.