#
# Filename: ImportCrmData.ps1
#
$ErrorActionPreference = "Stop"
$InformationPreference = "Continue"

$D365ConnectionString = "AuthType=AD;Url=https://contoso:8080/Test;";
if (Get-Variable 'D365_IMPORTENV_CONNECTIONSTRING' -ErrorAction 'Ignore') {
    #This allows me to set connection strings in my $profile and not accidently commit them
    $D365ConnectionString = $D365_IMPORTENV_CONNECTIONSTRING
}

#Load XrmCIFramework Extensions
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$xrmCIToolkit = $scriptPath + "\Xrm.Framework.CI.Extensions.PowerShell.Cmdlets.dll"
Import-Module $xrmCIToolkit

#Call commandlet to actually export data
Export-XrmData -ConnectionString $D365ConnectionString -FetchXmlPath .\fetchquery\export_config.xml -DataFilePath .\sampledata\il_configsetting.json -Verbose