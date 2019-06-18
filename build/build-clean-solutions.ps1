#Setup - this script should work both locally and on TFS Pipeline
#This script builds and tags all binaries on the build server

#Stop on first error
$ErrorActionPreference = "Stop"

#Execute Script to set build variables
. .\"_resolve-build-tools.ps1"

#Resolve Solution Path
$buildFolder = (Resolve-Path ..\src).Path

#Clean Solution
#delete all bin and obj folders
LogMessage "Cleaning Solutions"
Get-ChildItem $("$buildFolder") -Recurse | Where-Object {$_.PSIsContainer -and $_.name -match '^obj$|^bin$'  -and $_.FullName -notmatch 'package'} | Remove-Item -Force -Recurse

#Execute MSBUILD
& $msbuild "$buildFolder\Xrm.Framework.CI.Extensions.sln" /t:Clean /verbosity:quiet
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }

#Tag all releaseable binaries with version and build info
& .\update_assemblyinfo.ps1 $buildFolder\Xrm.Framework.CI.Extensions\Properties\AssemblyInfo.cs Xrm.Framework.CI.Extensions
& .\update_assemblyinfo.ps1 $buildFolder\Xrm.Framework.CI.Extensions.Tests\Properties\AssemblyInfo.cs Xrm.Framework.CI.Extensions.Tests

