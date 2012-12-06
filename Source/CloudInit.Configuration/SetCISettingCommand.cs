//-----------------------------------------------------------------------
// <copyright file="SetCISettingCommand.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace CloudInit.Configuration
{
    /// <summary>
    /// This command sets the settings in the registry
    /// <example>
    /// Set-CISetting -Name RunOnce -Value 1
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "CISetting")]
    public class SetCISettingCommand : Cmdlet
    {
        private String name = String.Empty;
        private Object value = null;

        /// <summary>
        /// Sets the name of the setting
        /// </summary>
        [Parameter(HelpMessage="Sets the name of the setting", Mandatory=true, Position=0)]
        public String Name { set { this.name = value; } }
        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        [Parameter(HelpMessage="Sets the value of the setting.", Mandatory = true, Position = 1)]
        public Object Value { set { this.value = value; } }

        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            try
            {
                // Create an instance of the registry object
                RegistryKey subKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\CloudInit\\Settings", true);

                // List the settings that are currently in the registry to make sure 
                // that they're updating a setting that actually exists
                List<String> settings = new List<String>(subKey.GetValueNames());

                // First make sure the setting exists, we don't want them typing in the name of the setting wrong
                if (settings.Exists((s) => { return s.CompareTo(this.name) == 0; }))
                {
                    subKey.SetValue(this.name, this.value);
                    WriteObject("OK-SettingUpdated");
                }
                else
                {
                    // Invalid name so throw an exception
                    throw new ArgumentException("Invalid CloudInit setting", "Name");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, String.Empty, ErrorCategory.InvalidOperation, this));
            }
        }
    }
}
