@echo OFF
REM requires msbuild (Vs2017+) to be in path
REM requires icogen and vpatch in path

set /p version="Enter SemVer: "
set /p appCode="Enter new android appCode: "

vpatch.exe -v %version% ^
            -c "src\GlobalAssemblyInfo.cs" ^
            -i "src\ShareEmergencyContacts.iOS\Info.plist" ^
            -u "src\ShareEmergencyContacts.UWP\Package.appxmanifest" ^
            -a "src\ShareEmergencyContacts.Android\Properties\AndroidManifest.xml" ^
            --appCode %appcode%

icogen.exe -i icons\android.icogen
icogen.exe -i icons\uwp.icogen
icogen.exe -i icons\iOS.icogen

REM Android
echo
echo
echo Automated build would be too easy. For now you have to follow these steps:
echo For Android select Release mode and build. Then open "Build -> Archive..", select the generated archive and save it with the certificate created.

echo For iOS/UWP: TODO
REM does build but creates two apks (1 signed, one unsigned) unsigned is "invalid format" and signed uses generic key
REM msbuild "src\ShareEmergencyContacts.Android\ShareEmergencyContacts.Android.csproj" /t:SignAndroidPackage "/p:Configuration=Release;Platform=Any CPU"

REM UWP has expired key and needs to be linked to store first
REM msbuild src\ShareEmergencyContacts.UWP\ShareEmergencyContacts.UWP.csproj /p:Configuration=Release;AppxBundle=Always;AppxBundlePlatforms="x86|x64|ARM" /p:BuildAppxUploadPackageForUap=true

REM needs mac build agent to run
REM msbuild "src\ShareEmergencyContacts.iOS\ShareEmergencyContacts.iOS.csproj" /t:Build "/p:Configuration=Release;Platform=iPhone"
PAUSE