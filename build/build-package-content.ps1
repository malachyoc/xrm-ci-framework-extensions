#Setup - this script should work both locally and on Azure Pipelines VM

#Stop on first error
$ErrorActionPreference = "Stop"

#Execute Script to set build variables
. .\"_resolve-build-tools.ps1"

#WebConfigTransformRunner Config Transformation
& $nugetExe install WebConfigTransformRunner -Version 1.0.0.1 -OutputDirectory .\packages -Verbosity quiet 
$webConfigTransformRunnerExe = ".\WebConfigTransformRunner.1.0.0.1\Tools\WebConfigTransformRunner.exe";

#Resolve Solution Path
$buildFolder = (Resolve-Path ..\).Path

#Create Folders where website content will be copied to
$psCommandletFolder = "$BUILD_ArtifactStagingDirectory\powershell";
rm $psCommandletFolder -r -fo -ErrorAction Ignore
$temp = md $psCommandletFolder

