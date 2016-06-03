@echo off
SETLOCAL

SET TASK=%1
SET CONFIG=%2
SET VERSION=%3
SET PRERELEASE=%4

IF [%1] == [] SET TASK=BUILD
IF [%2] == [] SET CONFIG=Debug
IF [%3] == [] SET VERSION=1.0.0
IF [%4] == [] SET PRERELEASE=pre03

IF [%1] == [help] GOTO DisplayHelp
if [%3] == [] GOTO BuildWithOutVersion
GOTO BuildWithVersion

:BuildWithVersion
IF [%3] == [] GOTO DisplayVersionError
IF [%4] == [] GOTO DisplayVersionError
echo Running psake build process with a specific Version (%VERSION%-%PRERELEASE%)
powershell.exe -command ".\psake.build.ps1 %TASK% -configuration %CONFIG% -packageVersion %VERSION% -preReleaseNumber %PRERELEASE% -updateNuspecFile $true -updateNuspecVersion $true"
GOTO Done

:BuildWithOutVersion
powershell.exe -command ".\psake.build.ps1 %TASK% -configuration %CONFIG%"
GOTO Done

:DisplayHelp
echo XLabs Build Script v1.0
echo Valid Paramters (postional based):
echo   1 - Task (default: Build) : The psake task to execute
echo          Clean: Cleans all intermediate files
echo          Build: Builds entire library
echo          Test: Executes unit tests
echo          Package: Builds the nuget packages
echo          Publish: Publishes nuget packages to nuget.org
echo          Increment-Version: Increment the preRelease version if it is in the version.json, if not it will increment the build version number
echo   2 - Configuration (default: Debug) : 
echo   3 - Version (optional): A full version number (ex: 2.2.0)
echo   4 - Prerelease (optional): A prerelease num (ex: pre01)
echo.
echo Examples:
echo     .\build.bat Clean
echo     .\build.bat Build %CONFIG%
echo     .\build.bat Build %CONFIG% %VERSION% %PRERELEASE%
echo.
GOTO Done

:DisplayVersionError
echo Version and Prerelease parameters must be provided.  Please try again...
GOTO Done

:Done
ENDLOCAL
