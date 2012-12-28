//--------------------------------------------------------------------------------------------
//   Copyright 2011 Brian Wight
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//--------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Management.Automation;
using CloudInit.Notification.Core;
using Microsoft.Win32;

namespace CloudInit.Configuration
{    
    /// <summary>
    /// This command sets the settings in the registry for a notification provider
    /// <example>
    /// Set-CINotificationSetting -ProviderName EmailNotification -Name ToEmail -Value nobody@nowhere.com
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "CINotificationSetting")]
    public class SetCINotificationSettingCommand : Cmdlet
    {
        private String providerName = String.Empty;
        private String name = String.Empty;
        private Object value = null;

        /// <summary>
        /// Sets the name of the provider
        /// </summary>
        [Parameter(HelpMessage="Sets the name of the provider", Mandatory=true, Position=0)]
        public String ProviderName { set { this.providerName = value; } }
        /// <summary>
        /// Sets the name of the setting
        /// </summary>
        [Parameter(HelpMessage="Sets the name of the setting", Mandatory = true, Position = 1)]
        public String Name { set { this.name = value; } }
        /// <summary>
        /// Sets the value of the setting
        /// </summary>
        [Parameter(HelpMessage="Sets the value of the setting", Mandatory = true, Position = 2)]
        public Object Value { set { this.value = value; } }

        /// <summary>
        /// Begins the processing
        /// </summary>
        protected override void BeginProcessing()
        {
            try
            {
                // Get the directory where the service is installed
                RegistryKey subKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\CloudInit");
                String imagePath = subKey.GetValue("ImagePath") as String;
                String directory = imagePath.Replace("CloudInit.exe", String.Empty);

                // Load all the assemblies and scan the types
                CIAssemblyLoader.Configure(directory);
                
                foreach (var type in CIAssemblyLoader.ScanFor<INotificationProvider>())
                {
                    INotificationProvider obj = Activator.CreateInstance(type) as INotificationProvider;
                    if (obj.ProviderName == this.providerName)
                    {
                        RegistryKey settingKey = Registry.LocalMachine.OpenSubKey(obj.SettingsPath, true);
                        List<String> settings = new List<String>(settingKey.GetValueNames());

                        // First make sure the setting exists, we don't want them typing in the name of the setting wrong
                        if (settings.Exists((s) => { return s.CompareTo(this.name) == 0; }))
                        {
                            settingKey.SetValue(this.name, this.value);
                            WriteObject("OK-SettingUpdated");
                        }
                        else
                        {
                            // Invalid name so throw an exception
                            throw new ArgumentException("Invalid CloudInit setting", "Name");
                        }

                        return;
                    }
                }

                // Provider was not found
                WriteObject("Error-NoProviderFound");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, String.Empty, ErrorCategory.InvalidOperation, this));
            }           
        }
    }
}
