#Setup - this script should work both locally and on Build Server

#Stop on first error
$ErrorActionPreference = "Stop"
cls;

#Start Stopwatch
$stopwatch =  [system.diagnostics.stopwatch]::StartNew()

#Child Scripts for build
$script_setBuildVariables = ".\_resolve-build-tools.ps1";
$script_cleanSolutions = ".\build-clean-solutions.ps1";
$script_buildContent = ".\build-compile-content.ps1";
$script_executeTests = ".\build-execute-tests.ps1";
$script_packageContent = ".\build-package-content.ps1";

#Load Global Build Variables and Functions
. $script_setBuildVariables
LogMessage "Build Variable script run: $script_setBuildVariables" Green

#Echo Build Variables
LogMessage "Build.ArtifactStagingDirectory: $BUILD_ArtifactStagingDirectory"
LogMessage "Build.BuildNumber: $BUILD_BuildNumber"
LogMessage "Build.QueuedBy: $BUILD_QueuedBy"
LogMessage "Build.SourceVersionMessage: $BUILD_SourceVersionMessage"
LogMessage "Build.SourceBranch: $BUILD_Branch"
LogMessage "Running on build server: $(!$BUILD_IsLocal)"
LogMessage "Executing As: $($env:UserName)"

#Recreate the $build-output folder to ensure a clean run locally
if($BUILD_IsLocal)
{
	LogMessage "Build Artifact Directory: $BUILD_ArtifactStagingDirectory"
	rm -r -fo $BUILD_ArtifactStagingDirectory -ErrorAction Ignore
	$temp = md $BUILD_ArtifactStagingDirectory
}

#Call component build scripts
LogMessage "Executing Clean Solutions Script: $script_cleanSolutions" Green
& $script_cleanSolutions

#Call component build scripts
LogMessage "Executing Building Content Script: $script_buildContent" Green
& $script_buildContent

#Run unit tests
LogMessage "Executing Test Scripts: $script_executeTests" Green
& $script_executeTests

LogMessage "Execute Packaging Content Script: $script_packageContent" Green
& $script_packageContent

LogMessage "Build took $($stopwatch.Elapsed)"