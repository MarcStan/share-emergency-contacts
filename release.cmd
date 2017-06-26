@echo OFF
REM requires msbuild (Vs2017+) and dotnet core 1.1 (msbuild.exe and dotnet.exe must both be in PATH)

set /p version="Enter SemVer: "
set /p appCode="Enter new android appCode: "
cd scripts
call "set version.cmd" %version% %appCode%
cd ..
echo "Not yet implemented"
PAUSE