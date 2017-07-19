@echo OFF
REM patches all required files with the provided version
REM call with arguments:
REM arg1: version number (semVer format), e.g. 1.2.3, no revision number!
REM arg2: Either ++ to increment android appCode by 1 or specific int to set to If left blank, appcode won't be edited
if "%1"=="" (
  echo "Missing parameter semVer"
  goto :eof
)
if not "%2"=="" (
  set appcode=--appCode=%2
)

nuget restore ..\src\VersionPatcher\VersionPatcher.sln
msbuild ..\src\VersionPatcher\VersionPatcher.sln /t:Build /p:Configuration=Release /nr:false

..\src\VersionPatcher\bin\Release\VersionPatcher.exe -v %1 ^
                                                            -c ..\src\GlobalAssemblyInfo.cs ^
                                                            -i "..\src\ShareEmergencyContacts.iOS\Info.plist" ^
                                                            -u "..\src\ShareEmergencyContacts.UWP\Package.appxmanifest" ^
                                                            -a "..\src\ShareEmergencyContacts.Android\Properties\AndroidManifest.xml" %appcode%
