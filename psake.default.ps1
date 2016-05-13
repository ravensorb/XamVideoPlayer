properties {
    [string]$configuration = "Release"
	[string]$packageVersion = $null
	[string]$preReleaseNumber = $null
    [string]$solution = $null
    [string]$source_folder = "."
	[string]$test_folder = "."
    [string]$deploy_folder = "deploy"
	[string]$nuget_folder = ".nuget"
	[string]$nuspec_folder = $null
	[string]$nuproj_folder = $null
    $projects = $null
	[bool]$updateVersion = $true
	[bool]$updateNuspecVersion = $true
	[bool]$updateNuspecFile = $true
	[string]$unittest_framework = "nunit"
	[string]$macAgentServerAddress = $null
	[string]$macAgentUser = $null
	[string]$baseNamespace = $null
	[bool]$processNuProjOutput = $false
	[bool]$autoIncrementVersion = $false
	[string]$nugetServerUrl = "https://nuget.org"
	[string]$nugetAPIKey = $null
}

Task Default -Depends DisplayParams,Build

Task DisplayParams -Depends Get-Version {
    Write-Host "`tconfiguration : $configuration"
	Write-Host "`tpackageVersion : $script:packageVersion"
	Write-Host "`tpreReleaseNumber : $script:preReleaseNumber"
    Write-Host "`tsolution : $solution"
	Write-Host "`tbaseNamespace: $baseNamespace"
    Write-Host "`tsource_folder : $source_folder"
	Write-Host "`ttest_folder : $test_folder"
    Write-Host "`tdeploy_folder : $deploy_folder"
	Write-Host "`tnuget_folder : $nuget_folder"
	Write-Host "`tnuspec_folder : $nuspec_folder"
	Write-Host "`tnuproj_folder : $nuproj_folder"
	Write-Host "`tupdateVersion : $updateVersion"
	Write-Host "`tupdateNuspecVersion : $updateNuspecVersion"
	Write-Host "`tupdateNuspecFile : $updateNuspecFile"
	Write-Host "`tunittest_framework : $unittest_framework"
	Write-Host "`tmacAgentServerAddress : $macAgentServerAddress"
	Write-Host "`tmacAgentUser : $macAgentUser"
    Write-Host "`tnugetServerUrl : $nugetServerUrl"
    Write-Host "`tnugetAPIKey : $nugetAPIKey"
    Write-Host "`tprojects : $projects"
}

Task Publish -Depends Get-Version,DisplayParams {
	$nugetConfig = Resolve-Path "$nuget_folder\nuget.config"

	Get-ChildItem -Path $deploy_folder | Where-Object -FilterScript {
		($_.Name.Contains("$project.$script:packageVersion")) -and !($_.Name.Contains(".symbols")) -and ($_.Extension -eq '.nupkg')    
	} | % {
		Write-Host "Publishing $($_.Fullname)"
		
		if ($nugetAPIKey)
		{
			exec { & "$nuget_folder\nuget.exe" "push" "$($_.Fullname)" -ConfigFile $nugetConfig -Source $nugetServerUrl -ApiKey $nugetAPIKey }
		} else {
			exec { & "$nuget_folder\nuget.exe" "push" "$($_.Fullname)" -ConfigFile $nugetConfig }
		}
	}
}

Task UnPublish -Depends Get-Version,DisplayParams {
	$ver = $script:packageVersion
	
	if ($script:preReleaseNumber)
	{
		$ver = "$ver-pre$($script:preReleaseNumber)"
	}

	$nugetConfig = Resolve-Path "$nuget_folder\nuget.config"

	Get-ChildItem -Path "$nuspec_folder\*.nuspec" -ErrorAction SilentlyContinue | % {
		Write-Host "`tUnPublishing $($_.BaseName) $ver"
		
		if ($nugetAPIKey)
		{
			exec { & "$nuget_folder\nuget.exe" "delete" "$($_.BaseName)" $ver -ConfigFile $nugetConfig -Source $nugetServerUrl -ApiKey $nugetAPIKey -NonInteractive }
		} else {
			exec { & "$nuget_folder\nuget.exe" "delete" "$($_.BaseName)" $ver -ConfigFile $nugetConfig -NonInteractive }
		}
	}
}

Task Package -Depends Get-Version,DisplayParams,RestoreDependencies { #-Depends Test {
	$ver = $script:packageVersion
	
	if ($script:preReleaseNumber)
	{
		$ver = "$ver-pre$($script:preReleaseNumber)"
	}
	
	New-Item $deploy_folder -ItemType Directory -ErrorAction SilentlyContinue	
	$path = Resolve-Path $deploy_folder
	
	Get-ChildItem -Path "$nuspec_folder\*.nuspec" -ErrorAction SilentlyContinue | % {
		$nuspecFile = $_;		
		$nuSpecFilePathTmp = $null;
		
		if ($updateNuspecVersion) 
		{ 
			if (-Not ($updateNuspecFile)) { $nuSpecFilePathTmp = "$nuspecFile.tmp.NuSpec"; }
			
			ChangeNuSpecVersion $baseNamespace $nuSpecFile $ver $nuSpecFilePathTmp $true
			
			if (-Not ($updateNuspecFile)) { $nuspecFile = $nuSpecFilePathTmp; }
		}

		Write-Host "`tnuspecFile: $nuSpecFile"
		Write-Host "`tpath: $path"
		Write-Host "`tver: $ver"
		Write-Host "`tconfiguration: $configuration"
		
		Try {
			exec { & "$nuget_folder\nuget.exe" pack $nuspecFile -OutputDirectory "$path" -MSBuildVersion 14 -Version $ver -Symbols -Prop Configuration=$configuration -Verbosity detailed }
		}
		Catch 
		{
			Write-Host "`tFailed generating package: $nuspecFile" -ForegroundColor Red
			Write-Host "`tError: $($_.Exception.Message)" -ForegroundColor Red
		}

		if (-Not ($updateNuspecFile)) { Remove-Item $nuSpecFilePathTmp -ErrorAction SilentlyContinue }
	}
}

Task Test -Depends Build {
	Get-ChildItem $source_folder -Recurse -Include *NUnit.csproj,*Test.csproj | % {
		switch ($unittest_framework) {
			"nunit" {
				Exec { & "xunit.console.exe" "$($_.DirectoryName)\bin\$configuration\$($_.BaseName).dll" }
			}
			"xunit" {
				Exec { & "nunit.console.exe" "$($_.DirectoryName)\bin\$configuration\$($_.BaseName).dll" }
			}
		}
	}
}

Task Build -Depends DisplayParams,Set-Versions,RestorePackages,RestoreDependencies {
	Exec { msbuild "$source_folder\$solution" /t:Build /p:Configuration=$configuration /consoleloggerparameters:"ErrorsOnly;WarningsOnly" /p:ServerAddress=$macAgentServerAddress /p:ServerUser=$macAgentUser } 
}

Task Clean -Depends DisplayParams {
	Exec { msbuild "$source_folder\$solution" /t:Clean /p:Configuration=$configuration /consoleloggerparameters:"ErrorsOnly" /p:ServerAddress=$macAgentServerAddress /p:ServerUser=$macAgentUser } 
	
	gci -Path $source_folder,$test_folder,$deploy_folder,$nuspec_folder,$nuproj_folder -Recurse -include 'bin','obj' -ErrorAction SilentlyContinue | % {
		remove-item $_ -recurse -force
		write-host deleted $_
	}
}

Task RestorePackages {
	New-Item -ItemType Directory  "$source_folder\packages" -ErrorAction SilentlyContinue
	$pathToPackages = Resolve-Path "$source_folder\packages"
	$nugetConfig = Resolve-Path "$nuget_folder\nuget.config"

	Write-Host "`tRestoring Packages to: $pathToPackages" -ForegroundColor Yellow
	Write-Host "`tUsing Nuget Config File: $nugetConfig" -ForegroundColor Yellow

	Exec { & "$nuget_folder\nuget.exe" restore "$source_folder\$solution" -MSBuildVersion 14 -PackagesDirectory $pathToPackages -ConfigFile $nugetConfig }
	Exec { & "$nuget_folder\nuget.exe" install NuProj -OutputDirectory $pathToPackages -ConfigFile $nugetConfig -Prerelease }
}

Task RestoreDependencies {
#	switch ($unittest_framework)
#	{
#		"nunit" { choco install -y nunit }
#	}
}

Task ProcessNuProjNuSpecFiles -Precondition { return $processNuProjOutput } {
	pushd
	cd $nuproj_folder
	#exec { & ".\process.ps1" }
	popd
}

Task Set-Versions -Depends Get-Version {
	if (-Not $updateVersion) { return }
	
	Get-ChildItem -Recurse -Force | ? { $_.Name -eq "AssemblyInfo.cs" -or $_.Name -eq "AssemblyInfo.Shared.cs" } | % {
		(Get-Content $_.FullName) | % {
			($_ -replace 'AssemblyVersion\(.*\)', ('AssemblyVersion("' + $script:packageVersion + '")')) -replace 'AssemblyFileVersion\(.*\)', ('AssemblyFileVersion("' + $script:packageVersion + '")')
		} | Set-Content $_.FullName -Encoding UTF8
	}    
}

Task Get-Version {
	if (-Not $script:packageVersion -and -Not $packageVersion) {
		$versionInfo = (Get-Content "version.json") -join "`n" | ConvertFrom-Json
		$script:packageVersion = "$($versionInfo.major).$($versionInfo.minor).$($versionInfo.build)";
		if ($versionInfo.preRelease) {
			$script:preReleaseNumber = "{0:00}" -f $versionInfo.preRelease
		}
		
		Write-Host "`tVersion Loaded: $script:packageVersion $script:preReleaseNumber"
	} else {
		$script:packageVersion = $packageVersion
		
		if ($preReleaseNumber) {
			$script:preReleaseNumber = "{0:00}" -f $preReleaseNumber
		}
		
		Write-Host "`tVersion Passed in: $script:packageVersion $script:preReleaseNumber"
	}
}

Task Increment-Version {
	if (-Not (Test-Path "version.json")) {
		$versionInfo = '{ "major":  2, "minor":  2, "build":  0, "preRelease":  1 }' | ConvertFrom-Json
	} else {
		$versionInfo = (Get-Content "version.json") -join "`n" | ConvertFrom-Json
	}

	$preVersion = "$($versionInfo.major).$($versionInfo.minor).$($versionInfo.build)";	
	
	if ($versionInfo.preRelease) {
		$preVersion = "$preVersion-pre" + "{0:00}" -f $versionInfo.preRelease
		$versionInfo.preRelease = [int]$versionInfo.preRelease + 1
		$newVersion = "-pre" + "{0:00}" -f $versionInfo.preRelease
	} else {
		$versionInfo.build = [int]$versionInfo.build + 1
	}
		
	$newVersion = "$($versionInfo.major).$($versionInfo.minor).$($versionInfo.build)" + $newVersion;	
	
	Write-Host "`tIncrementing Version from $preVersion to $newVersion"
	$versionInfo | ConvertTo-Json | Out-File "version.json"
}

#######################################################################################################################

function ChangeNuSpecVersion([string] $baseNamespace, [string] $nuSpecFilePath, [string] $versionNumber="0.0.0.0", [string] $nuSpecFilePathTmp = $null, [bool] $dependencyupdateVersion = $false, [string] $dependencyVersion = $null)
{
	Write-Host "`tDynamically setting NuSpec version: $versionNumber" -ForegroundColor Yellow
	Write-Host "`t`tBase Namespace: $baseNamespace" -ForegroundColor Yellow
	Write-Host "`t`tNuspec File: $nuSpecFilePath" -ForegroundColor Yellow
	Write-Host "`t`tTemp Nuspec File: $nuSpecFilePathTmp" -ForegroundColor Yellow
	Write-Host "`t`tDependency Version: $dependencyVersion" -ForegroundColor Yellow
	Write-Host "`t`tUpdate Dependency Version: $dependencyupdateVersion" -ForegroundColor Yellow
	
	# Get full path or save operation fails when launched in standalone powershell
	$nuSpecFile = Get-Item $nuSpecFilePath | Select-Object -First 1
	
	# Bring the XML Linq namespace in
	[Reflection.Assembly]::LoadWithPartialName( "System.Xml.Linq" ) | Out-Null
	
	# Update the XML document with the new version	
	$xmlns = [System.Xml.Linq.XNamespace] "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd";
	$xDoc = [System.Xml.Linq.XDocument]::Load( $nuSpecFile.FullName )
	$versionNode = $xDoc.Descendants( $xmlns + "version" ) | Select-Object -First 1
	if ($versionNode -ne $null)
	{
		Write-Host "`t`t`tUpdating Version: $versionNumber" -ForegroundColor Green
		$versionNode.SetValue($versionNumber)
	}
	
	# Update the XML document dependencies with the new version
	if ($dependencyupdateVersion)
	{
		Write-Host "`tUpdating Dependencies" -ForegroundColor Yellow
		$dependencies = $xDoc.Descendants( $xmlns + "dependency" )
		foreach( $dependency in $dependencies )
		{
			$idAttribute = $dependency.Attributes( "id" ) | Select-Object -First 1
			Write-Host "`t`tDependency: $idAttribute"
			if ( $idAttribute -ne $null -or $idAttribute)
			{
				if ($baseNamespace -and $idAttribute.Value -like "$baseNamespace.*" )
				{
					Write-Host "`t`t`tUpdating Dependency Version: $versionNumber" -ForegroundColor Green

					if ($dependencyVersion)					
					{
						$dependency.SetAttributeValue( "version", "$dependencyVersion" )
					} 
					else
					{
					$dependency.SetAttributeValue( "version", "[$versionNumber]" )
					}
				}
			}
		}
	}
	
	# Save file
	if ($nuSpecFilePathTmp) 
	{ 
		Write-Host "`tCreating a temporary NuSpec file with the new version" -ForegroundColor Yellow
		$xDoc.Save( $nuSpecFilePathTmp, [System.Xml.Linq.SaveOptions]::None )
	} else {
		$xDoc.Save( $nuSpecFile.FullName, [System.Xml.Linq.SaveOptions]::None )
	}
}