@echo OFF

nuget restore ..\src\IconGenerator\IconGenerator.sln
msbuild ..\src\IconGenerator\IconGenerator.csproj /t:Build /p:Configuration=Release /nr:false
..\src\IconGenerator\bin\Release\IconGenerator.exe -i ..\icons\android.icogen
..\src\IconGenerator\bin\Release\IconGenerator.exe -i ..\icons\uwp.icogen
..\src\IconGenerator\bin\Release\IconGenerator.exe -i ..\icons\iOS.icogen