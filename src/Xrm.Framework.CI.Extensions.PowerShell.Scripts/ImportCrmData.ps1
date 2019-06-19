#
# Filename: ImportCrmData.ps1
#
[CmdletBinding()]
param(
	[string]$crmConnectionString #The target CRM organization connection string
	, [string]$dataFilename #Optional - will use this as import log file name
	, [string]$logFilename #Optional - will use this as import log file name
)

$ErrorActionPreference = "Stop"
$InformationPreference = "Continue"

Write-Verbose 'Entering ImportCrmData.ps1'

#Script Location
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Write-Verbose "Script Path: $scriptPath"

#Load XrmCIFramework
$xrmCIToolkit = $scriptPath + "\Xrm.Framework.CI.Extensions.PowerShell.Cmdlets.dll"
Write-Verbose "Importing CIToolkitExtensions: $xrmCIToolkit" 
Import-Module $xrmCIToolkit
Write-Verbose "Imported CIToolkitExtensions"

Write-Verbose "dataFilename = $dataFilename"
Write-Verbose "crmConnectionString = $crmConnectionString"
Write-Verbose "logFilename = $logFilename"

Import-XrmData -ConnectionString "$CrmConnectionString" -DataFilePath "$dataFilename"
 
Write-Verbose 'Leaving ImportCrmData.ps1'