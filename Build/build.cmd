@echo off
SETLOCAL
SET SCRIPT_PATH=%~dp0
SET CACHED_NUGET=%LocalAppData%\NuGet\NuGet.exe
SET NUGET_DIR=%SCRIPT_PATH%\.nuget
SET PACKAGE_DIR=%SCRIPT_PATH%\packages

IF EXIST %CACHED_NUGET% goto copynuget
echo Downloading latest version of NuGet.exe...
IF NOT EXIST %LocalAppData%\NuGet md %LocalAppData%\NuGet
@powershell -NoProfile -ExecutionPolicy unrestricted -Command "$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest 'https://www.nuget.org/nuget.exe' -OutFile '%CACHED_NUGET%'"

:copynuget
IF EXIST %NUGET_DIR%\nuget.exe goto restore
md %NUGET_DIR%
copy %CACHED_NUGET% %NUGET_DIR% > nul

:restore

%NUGET_DIR%\NuGet.exe update -self
%NUGET_DIR%\NuGet.exe install FAKE -OutputDirectory %PACKAGE_DIR% -ExcludeVersion -Version 4.61.3
%NUGET_DIR%\NuGet.exe install FSharp.Data -OutputDirectory %PACKAGE_DIR%\FAKE -ExcludeVersion -Version 2.3.3

SET encoding=utf-8
%PACKAGE_DIR%\FAKE\tools\FAKE.exe Build\Fake\build.fsx %*
