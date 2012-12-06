//-----------------------------------------------------------------------
// <copyright file="GetCINotificationProvidersCommand.cs" company="SBR Net Marketing LLC" author="Brian Wight">
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
    /// This command gets a list of available NotificationProviders
    /// <example>
    /// Get-CINotificationProviders</example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "CINotificationProviders")]
    public class GetCINotificationProvidersCommand : Cmdlet
    {
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
                    providers.Add(obj.ProviderName, type);
                }

                WriteObject(providers);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, String.Empty, ErrorCategory.InvalidOperation, this));
            }
        }
    }
}
