#Setup - this script should work both locally and on TFS Pipeline
#This script builds and tags all binaries on the build server

#Stop on first error
$ErrorActionPreference = "Stop"

#Execute Script to set build variables
. .\"_resolve-build-tools.ps1"

#Resolve Solution Path
$buildFolder = (Resolve-Path ..\src).Path

#Tag all releaseable binaries with version and build info
& .\update_assemblyinfo.ps1 $buildFolder\Xrm.Framework.CI.Extensions\Properties\AssemblyInfo.cs Xrm.Framework.CI.Extensions
& .\update_assemblyinfo.ps1 $buildFolder\Xrm.Framework.CI.Extensions.Tests\Properties\AssemblyInfo.cs Xrm.Framework.CI.Extensions.Tests

#Restore NuGet Packages
LogMessage "Restoring NuGet Packages for: $buildFolder\Xrm.Framework.CI.Extensions.sln"
& $nugetExe restore $buildFolder\Xrm.Framework.CI.Extensions.sln -PackagesDirectory $buildFolder\packages -Verbosity quiet
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }

& $nugetExe restore $buildFolder\Xrm.Framework.CI.Extensions -PackagesDirectory $buildFolder\packages -Verbosity quiet
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }
& $nugetExe restore $buildFolder\Xrm.Framework.CI.Extensions.Tests -PackagesDirectory $buildFolder\packages -Verbosity quiet
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }

#build projects
LogMessage "Debug Build: $buildFolder\Xrm.Framework.CI.Extensions.sln"
& $msbuild $buildFolder\Xrm.Framework.CI.Extensions.sln /property:Configuration=Debug /verbosity:quiet
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }

LogMessage "Release Build: $buildFolder\Xrm.Framework.CI.Extensions.sln"
& $msbuild $buildFolder\Xrm.Framework.CI.Extensions.sln /property:Configuration=Release /verbosity:quiet
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }
