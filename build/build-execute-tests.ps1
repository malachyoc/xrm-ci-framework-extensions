#Setup - this script should work both locally and on Build Server
#Stop on first error
$ErrorActionPreference = "Stop"

#Execute Script to set build variables
. .\"_resolve-build-tools.ps1"

#TODO: Set build relevent variables
. $nugetExe install xunit.runner.console -OutputDirectory .\packages -Version 2.4.1 
. $nugetExe install OpenCover -OutputDirectory .\packages -Version 4.7.922 
. $nugetExe install ReportGenerator -OutputDirectory .\packages -Version 4.1.4 
. $nugetExe install OpenCoverToCoberturaConverter -OutputDirectory .\packages -Version 0.3.4 

$xunitExe = ".\packages\xunit.runner.console.2.4.1\tools\net462\xunit.console.exe";
$openCoverExe = ".\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe";
$reportGeneratorExe = ".\packages\ReportGenerator.4.1.4\tools\net47\ReportGenerator.exe";
$openCoverToCoberturaConverterExe = ".\packages\OpenCoverToCoberturaConverter.0.3.4\tools\OpenCoverToCoberturaConverter.exe";

#this script assumes that the content has been successfully build

#Local Build PAths
$openCoverOutputFolder = "$BUILD_ArtifactStagingDirectory\opencover";

#OpenCover executes the unit tests to get code coverage
rm $openCoverOutputFolder -r -fo  -ErrorAction Ignore
$temp = md $openCoverOutputFolder

#Set arguments for XUnit
$xunitArguments = "..\src\Xrm.Framework.CI.Extensions.Tests\bin\Debug\Xrm.Framework.CI.Extensions.Tests.dll" `
	+ " -noshadow -xml $openCoverOutputFolder\Xrm.Framework.CI.Extensions.Tests.xml"

#Run OpenCover
. $openCoverExe `
	-register:user `
	-output:$openCoverOutputFolder\Xrm.Framework.CI.Extensions.OpenCover.xml `
	-target:"$xunitExe" `
	-targetargs:$xunitArguments `
	-filter:"+[Xrm.Framework.CI.Extensions]* -[*.Test]*" `
	-log:warn
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }

#Generate HTML Coverage Report
. $reportGeneratorExe `
	-reports:$openCoverOutputFolder\*.OpenCover.xml `
	-reporttypes:Html `
	-targetdir:$openCoverOutputFolder\reports `
	-historydir:../src/_opencover-history `
	-verbosity:Info
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }

. $openCoverToCoberturaConverterExe `
	-input:$openCoverOutputFolder\Xrm.Framework.CI.Extensions.OpenCover.xml `
	-output:$openCoverOutputFolder\Xrm.Framework.CI.Extensions.Cobertura.xml
if($LASTEXITCODE -ne 0) { throw "BUILD STEP FAILED"; }