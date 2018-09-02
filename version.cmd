@echo OFF

set /p version="Enter SemVer: "
set /p appCode="Enter new android appCode (empty to skip): "

vpatch.exe -v %version% ^
            -c src\GlobalAssemblyInfo.cs ^
            -u src\ShareEmergencyContacts.UWP\Package.appxmanifest ^
            -i src\ShareEmergencyContacts.iOS\Info.plist ^
            -a src\ShareEmergencyContacts.Android\Properties\AndroidManifest.xml

If "%appCode%"=="" goto :eof

vpatch.exe -v %version% ^
            -a src\ShareEmergencyContacts.Android\Properties\AndroidManifest.xml ^
            --appCode %appcode%