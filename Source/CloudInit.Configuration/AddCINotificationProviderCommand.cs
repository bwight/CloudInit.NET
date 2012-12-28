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
using CloudInit.Notification.Core;
using Microsoft.Win32;

namespace CloudInit.Configuration
{
    /// <summary>
    /// This command adds a notification provider to the CloudInit service
    /// <example>
    /// Add-CINotificationProvider -ProviderName EmailNotification
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "CINotificationProvider")]
    public class AddCINotificationProviderCommand : Cmdlet
    {
        private String providerName = String.Empty;

        /// <summary>
        /// Sets the name of the provider to add.
        /// </summary>
        [Parameter( HelpMessage="Sets the name of the provider to add", Mandatory=true, Position=0)]
        public String ProviderName
        {
            set
            {
                this.providerName = value;
            }
        }

        /// <summary>
        /// Begins the processing.
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

                // Make a list of all the providers
                Hashtable providers = new Hashtable();

                // List all the types that inherit INotificationProvider
                foreach (var type in CIAssemblyLoader.ScanFor<INotificationProvider>())
                {
                    INotificationProvider obj = Activator.CreateInstance(type) as INotificationProvider;
                    if (obj.ProviderName == this.providerName)
                    {
                        obj.Add();
                        WriteObject("OK-ProviderAdded");
                        return;
                    }
                }

                // Could not find the provider
                WriteObject("Error-NoProviderFound");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, String.Empty, ErrorCategory.InvalidOperation, this));
            }
        }
    }
}
