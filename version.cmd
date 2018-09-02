@echo OFF
REM args are optional. If not set, interactive mode is started
REM [semVer] [androidCode]
set version=%1
if [%1]==[] (
    set /p version="Enter SemVer: "
)
set appCode=%2
if [%2]==[] (
    set /p appCode="Enter new android appCode (empty to skip): "
)

vpatch.exe -v %version% ^
            -c src\GlobalAssemblyInfo.cs ^
            -u src\ShareEmergencyContacts.UWP\Package.appxmanifest ^
            -i src\ShareEmergencyContacts.iOS\Info.plist ^
            -a src\ShareEmergencyContacts.Android\Properties\AndroidManifest.xml

If "%appCode%"=="" goto :eof

vpatch.exe -v %version% ^
            -a src\ShareEmergencyContacts.Android\Properties\AndroidManifest.xml ^
            --appCode %appcode%