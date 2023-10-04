function Check-Command($cmdname)
{
    if(Get-Command -Name $cmdname -ErrorAction SilentlyContinue) 
	{
		return $true;
	}
	else
	{
		return $false;
	}
}

function GetEnvironmentalVariable([String]$variableName, [String]$defaultValue)
{
	$upperVariable = $variableName.ToUpper().replace(".", "_");
	$targetVariable = "`$env:$upperVariable";

	$value = Invoke-Expression $targetVariable;
	if([string]::IsNullOrEmpty($value))
	{
		return $defaultValue;
	}
	else 
	{
		return $value;
	}
}

function LogMessage([String]$message, [String]$color)
{
	$messageTime = (get-date).ToString("yyyy-MM-dd HH:mm:ss");
	if([string]::IsNullOrEmpty($color))
	{
		Write-Host "[$messageTime]: $message"
	}
	else
	{
		Write-Host "[$messageTime]: $message" -ForegroundColor $color
	}
}

#set MSBuild location
$msbuild = "msbuild.exe";
if (!(Check-Command $msbuild))
{
	#MSBuild is not on the path; assume we are running on Azure Pipelines VM
	$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe";
	
	if (!(Check-Command $msbuild))
	{ 
		$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe";
	}
	
	if (!(Check-Command $msbuild))
	{ 
		#MSBuild is not found - should not happen
		Write-Error "$('[{0:MM/dd/yyyy} {0:HH:mm:ss}]' -f (Get-Date)): Could not find '$msbuild'.  MsBuild must be on your local path"
		exit 1
	}
}

#Set Relative Globals
$nugetExe = (Resolve-Path ..\lib\nuget.exe).Path

#Set Build Variables
$BUILD_ArtifactStagingDirectory = GetEnvironmentalVariable "Build.ArtifactStagingDirectory" "..\_build-output"
$BUILD_BuildNumber = GetEnvironmentalVariable "Build.BuildNumber" "MANUAL"
$BUILD_QueuedBy = GetEnvironmentalVariable "Build.QueuedBy" ([System.Security.Principal.WindowsIdentity]::GetCurrent().Name)
$BUILD_SourceVersionMessage = GetEnvironmentalVariable "Build.SourceVersionMessage" "LOCALBUILD"
$BUILD_Branch = GetEnvironmentalVariable "Build.SourceBranch" "LOCALBRANCH"

$BUILD_IsLocal = $BUILD_BuildNumber -eq "MANUAL";