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
using System.Collections;
using System.Management.Automation;
using System.Reflection;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace CloudInit.Configuration
{
    /// <summary>
    /// This command reads the settings from registry and displays the result to the user
    /// <example>
    /// Get-CISettingsCommand
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "CISettings")]
    public class GetCISettingsCommand : Cmdlet
    {
        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            try
            {
                // Create an instance of the registry object
                RegistryKey subKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\CloudInit\\Settings", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                Hashtable settings = new Hashtable();

                // Add the version to the settings list
                settings.Add("Version", Assembly.GetExecutingAssembly().GetName().Version);

                // Write all the settings in the settings folder
                foreach (String name in subKey.GetValueNames())
                {
                    settings.Add(name, subKey.GetValue(name));
                }

                WriteObject(settings);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, String.Empty, ErrorCategory.InvalidOperation, this));
            }
        }
    }
}
