##   Copyright 2011 Brian Wight
##
##   Licensed under the Apache License, Version 2.0 (the "License");  
##   you may not use this file except in compliance with the License.  
##   You may obtain a copy of the License at  
##
##       http://www.apache.org/licenses/LICENSE-2.0
##
##   Unless required by applicable law or agreed to in writing, software  
##   distributed under the License is distributed on an "AS IS" BASIS,  
##   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  
##   See the License for the specific language governing permissions and  
##   limitations under the License.  

<# 
.SYNOPSIS
This is a simple script to install and uninstall CloudInit.NET

.DESCRIPTION
The script will add two registry keys to hold settings for the CloudInit.NET service and install the CloudInit service.

.PARAMETER u
Setting this parameter to 1 will uninstall the service

#>

param([bool]$u = $false)

## User Settings 
## ** Important to note that for t1.micro instances the default userdatatimeout should be 
## greater than 40 seconds the performance of t1.micro instances varies and you can expect 
## downloading the userdata file to be greater than 30 seconds at times.
$userDataFile = 	"http://169.254.169.254/1.0/user-data"
$userDataTimeout = 	100000	
$logFile = 			"C:\CloudInit.log"
$secretKey = 		"3e3e2d3848336b7d3b547b2b55"
$runOnce =			1
$servicePath = 		pwd
$psModulePath = 	"$pshome\Modules\CloudInitAdministration"
$serviceUser =		"cloudinitservice"
$serviceUserPass = 	"vd65*l!_op"

if(!$u)
{
	## Create The user for this service
	net user $serviceUser $serviceUserPass /add
	## If you're using a domain you can replace these lines with
	net localgroup Administrators $serviceUser /add
	## Set the password to never expire
	WMIC USERACCOUNT WHERE "Name='$($serviceUser)'" SET PasswordExpires=FALSE
	
	
	## Set permissions for ServiceLogonRight for user
	$username = "$(hostname)\$($serviceUser)"
	[System.Reflection.Assembly]::LoadFrom("$servicePath\CloudInit.exe")
	if([CloudInit.LSAHelper]::AddPrivileges($username, "SeServiceLogonRight") -ne 0)
	{
		write-error "Error occurred while adding privileges to user account. You may need to manually set the Log on as Service right for user $serviceUser for CloudInit.NET to work properly"
	}
	
	## Create The Service 
	sc.exe create CloudInit binPath= "$servicePath\CloudInit.exe" DisplayName= "CloudInit.NET" start= delayed-auto obj= ".\$($serviceUser)" password= "$serviceUserPass"
	
	## Create Required Registry Key for user-data
	reg add HKLM\SYSTEM\CurrentControlSet\services\CloudInit\Settings /v UserDataFile /t REG_SZ /d $userDataFile
	reg add HKLM\SYSTEM\CurrentControlSet\services\CloudInit\Settings /v UserDataTimeout /t REG_DWORD /d $userDataTimeout
	reg add HKLM\SYSTEM\CurrentControlSet\services\CloudInit\Settings /v LogFile /t REG_SZ /d $logFile
	reg add HKLM\SYSTEM\CurrentControlSet\services\CloudInit\Settings /v SecretKey /t REG_SZ /d $secretKey
	reg add HKLM\SYSTEM\CurrentControlSet\services\CloudInit\Settings /v RunOnce /t REG_DWORD /d $runOnce
	
	## Create powershell module folder
	mkdir $psModulePath
	
	## Copy the required module dll's to the powershell module
	cp "$servicePath\CloudInit.Configuration.dll" "$psModulePath\CloudInit.Configuration.dll"
	cp "$servicePath\CloudInit.Notification.Core.dll" "$psModulePath\CloudInit.Notification.Core.dll"

	## Create a new manifest for the cloudinitadministration module
	new-modulemanifest -DotNetFrameworkVersion "4.0" -CLRVersion "4.0.30319" -Path "$psModulePath\cloudinitadministration.psd1" -Author "Brian Wight" -CompanyName "SBR Net Marketing" -Copyright "Copyright 2011 SBR Net Marketing LLC. All rights reserved" -Description "Modules for the CloudInit Service created by Brian Wight" -ModuleVersion 1.2 -RequiredAssemblies "$psModulePath\CloudInit.Notification.Core.dll" -TypesToProcess @() -NestedModules @() -ModuleToProcess "CloudInit.Configuration" -FormatsToProcess @() -FileList @()
}
else
{
	## Remove theu service
	sc.exe delete "CloudInit"
	
	## Clean up the user accounts
	net user $serviceUser /delete
	
	## Remove the powershell module
	Remove-Item -Recurse -Force $psModulePath
}

