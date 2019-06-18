Param(
	[Parameter(Mandatory = $true)][string]$assebmleyInfoFile
	, [Parameter(Mandatory = $false)][string]$assembleyTitle
)

$ErrorActionPreference = "Stop"

#https://stackoverflow.com/questions/13943456/what-is-the-formula-in-net-for-wildcard-version-numbers
#Copy Visual studio convention for build and revision
$PSDefaultParameterValues['Out-File:Encoding'] = 'utf8'
$PSDefaultParameterValues['*:Encoding'] = 'utf8'

$startDate = Get-Date -Date "2000-01-01"
$todayDate = Get-Date

$buildTimespan = New-TimeSpan -Start $startDate -End $todayDate
$revisionTimespan = New-TimeSpan -Start $todayDate.Date -End $todayDate

#load the project file
$fullProjectFilePath = resolve-path $assebmleyInfoFile

$build = $buildTimespan.Days
$revision = ($revisionTimespan.TotalSeconds / 2 ) -as [int]

$filecontent = Get-Content($assebmleyInfoFile)

# Regular expression pattern to find the version in the build number 
$assemblyVersionRegex = "(?<=AssemblyVersion\("")(\d+\.\d+\.\d+\.\d+)(?=""\))"
$assemblyFileVersionRegex = "(?<=AssemblyFileVersion\("")(\d+\.\d+\.\d+\.\d+)(?=""\))"
$assemblyTitleRegex = "(?<=AssemblyTitle\("")([^\n]+)(?=""\))"

#Globally Scoped Variable
$adjustedFileVersion; $adjustedAssembleyVersion;

if($filecontent -match $assemblyVersionRegex)
{
	$assemblyVersionVersion = ($filecontent | Select-String -Pattern $assemblyVersionRegex).Matches[0].Value
	$adjustedFileVersion = $assemblyVersionVersion.SubString(0, $assemblyVersionVersion.IndexOf(".", $assemblyVersionVersion.IndexOf(".") + 1)) + ".$build.$revision"

	$filecontent = $filecontent -replace $assemblyVersionVersion, $adjustedFileVersion
}

if($filecontent -match $assemblyFileVersionRegex)
{
	$fileVersion = ($filecontent | Select-String -Pattern $assemblyFileVersionRegex).Matches[0].Value
	$adjustedAssembleyVersion = $fileVersion.SubString(0, $fileVersion.IndexOf(".", $fileVersion.IndexOf(".") + 1)) + ".$build.$revision"

	$filecontent = $filecontent -replace $assemblyFileVersionRegex, $adjustedAssembleyVersion
}

if($filecontent -match $assemblyTitleRegex)
{
	$builder = ([string]$BUILD_QueuedBy).Replace("\", "\\");

	$adjustedAssembleyTitle = "$assembleyTitle [Branch: $BUILD_Branch ($BUILD_SourceVersionMessage)] [Build: $BUILD_BuildNumber] [Built By: $builder]";
	LogMessage "Tagging Assembly: $adjustedAssembleyTitle [File Version: $adjustedFileVersion]  [Assembly Version: $adjustedFileVersion]"
	$filecontent = $filecontent -replace $assemblyTitleRegex, $adjustedAssembleyTitle
}

$Utf8BomEncoding = New-Object System.Text.UTF8Encoding $False
[System.IO.File]::WriteAllLines($fullProjectFilePath, $filecontent, $Utf8BomEncoding) 
