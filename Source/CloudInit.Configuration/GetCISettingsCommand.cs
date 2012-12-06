//-----------------------------------------------------------------------
// <copyright file="GetCISettingsCommand.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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
