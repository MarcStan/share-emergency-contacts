@echo OFF
REM requires msbuild (Vs2017+) and dotnet core 1.1 (msbuild.exe and dotnet.exe must both be in PATH)

set /p version="Enter SemVer: "
set /p appCode="Enter new android appCode: "
cd scripts
call "generate icons.cmd"
call "set version.cmd" %version% %appCode%
cd ..

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