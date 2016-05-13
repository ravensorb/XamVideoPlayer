Param(
    [Parameter(Position=1,Mandatory=0)]
    [string[]]$task_list = @(),

	[Parameter()]
    [string]$packageVersion = $null,
	
	[Parameter()]
    [string]$preReleaseNumber = $null,
	
	[Parameter()]
    [string]$configuration = "Release",
	
	[Parameter()]
	[bool]$autoIncrementVersion = $false,
	
	[Parameter()]
	[bool]$updateVersion = $false,
		
	[Parameter()]
	[bool]$processNuProjOutput = $false,
	
	[Parameter()]
	[bool]$updateNuspecFile = $true,
	
	[Parameter()]
	[bool]$updateNuspecVersion = $true,
	
	[Parameter()]
	[string]$nugetAPIKey = $null
)

$build_file = 'psake.default.ps1'

# Properties for the psake build script
$properties = @{

    # Build configuration to use
    "configuration" = $configuration;

    # Version number to use if running the Publish build task.
    # This will be read from the command line args
    "packageVersion" = $packageVersion;

	# Is the Nuget package a pre-release version?
	"preReleaseNumber" = $preReleaseNumber;
	
	# Do we want to auto increment the version (if a prerelease it will increment prerelease otherwise it will increment build number)
	"autoIncrementVersion" = $autoIncrementVersion;

    # Path to the solution file
    "solution"      = 'Xam.Plugins.VideoPlayer.sln';
	
	# Base namespeace for the package (used for updating nuspec details)
	"baseNamespace" = 'Xam.Plugins.VideoPlayer';

    # Folder containing source code
    "source_folder" = '.\source';
	
	# Folder container unit tests
	"test_folder" = '.\tests';

    # Folder to output deployable packages to. This folder should be ignored
    # from any source control, as we dont commit build artifacts to source
    # control
    "deploy_folder" = '.\artifacts\packages';

	# Folder that contains nuget files (nuget.exe, nuget.config)
	"nuget_folder" = '.\.nuget';

	# Folder that contains nuspec files
	"nuspec_folder" = '.\.nuget\definitions';
	
	# Folder that contains nuspec files
	"nuproj_folder" = '.\.nuget\source';
	
	# List of projects to use when building NuGet Packages (Note: Not used for XLabs)
    "projects" = @(
	);
	
	# Unit Test Framework (nunit, xunit)
	"unittest_framework" = 'nunit';
	
	# Update the version numbers automatically
	"updateVersion" = $updateVersion;
	
	"updateNuspecVersion" = $updateNuspecVersion;
	
	"updateNuspecFile" = $updateNuspecFile;
	
	# The name or ip address of the Mac that is running the Xamarin build agent
	"macAgentServerAddress" = $null; 
	
	# The user name to use to authentice for the Xamarin build agent
	"macAgentUser" = $null; 
	
	"processNuProjOutput" = $processNuProjOutput;
	
	"nugetServerUrl" = 'https://nuget.org';
	
	"nugetAPIKey" = $null;
}

#if (!(Get-Module -Name psake -ListAvailable)) { Install-Module -Name psake -Scope CurrentUser }

#import-module .\packages\psake.4.4.2\tools\psake.psm1
import-module C:\ProgramData\chocolatey\lib\psake\tools\psake.psm1

invoke-psake $build_file $task_list -Properties $properties -Framework "4.6"

Write-Host "Done..."