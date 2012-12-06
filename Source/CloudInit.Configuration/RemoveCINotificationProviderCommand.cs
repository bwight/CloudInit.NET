//-----------------------------------------------------------------------
// <copyright file="RemoveCINotificationProviderCommand.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Management.Automation;
using CloudInit.Notification.Core;
using Microsoft.Win32;

namespace CloudInit.Configuration
{
    /// <summary>
    /// This command removes the notification provider
    /// <example>
    /// Remove-CINotificationProvider -ProviderName EmailNotification
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "CINotificationProvider")]
    public class RemoveCINotificationProviderCommand : Cmdlet
    {
        private String providerName = String.Empty;

        /// <summary>
        /// The name of the provider to remove
        /// </summary>
        [Parameter(HelpMessage = "The name of the provider to remove", Mandatory = true, Position = 0)]
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

                foreach (var type in CIAssemblyLoader.ScanFor<INotificationProvider>())
                {
                    INotificationProvider obj = Activator.CreateInstance(type) as INotificationProvider;
                    if (obj.ProviderName == this.providerName)
                    {
                        obj.Remove();
                        WriteObject("OK-ProviderRemoved");

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
