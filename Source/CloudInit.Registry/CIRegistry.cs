//-----------------------------------------------------------------------
// <copyright file="CIRegistry.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Security;
using System.Security.AccessControl;
using Microsoft.Win32;
using System.Globalization;

namespace CloudInit.Registry
{
    /// <summary>
    /// Interfaces with the Windows Registry to grab the settings for this service.
    /// </summary>
    public class CIRegistry
    {
        /// <summary>
        /// This is the EventLog where all errors should be logged
        /// </summary>
        private EventLog eventLog = null;

        /// <summary>
        /// Default value for user-data file if no value is found in the registry
        /// </summary>
        private String defaultUserDataFile = "http://169.254.169.254/1.0/user-data";

        /// <summary>
        /// Default value for the log file if no value is found in the registry
        /// </summary>
        private String defaultLogFile = "C:\\CloudInit.log";

        /// <summary>
        /// Default value for the Rijndael key if no value is found in the registry
        /// </summary>
        private String defaultSecureKey = "3e3e2d3848336b7d3b547b2b55";

        /// <summary>
        /// Default value for the user-data timeout if no value is found in the registry
        /// </summary>
        private Int32 defaultUserDataTimeout = 100000;

        /// <summary>
        /// Default value for IsBase64Encoded if no value is found in the registry
        /// </summary>
        private Boolean defaultIsBase64Encoded = false;

        /// <summary>
        /// Default value for the RunOnce key if no value is found in the registry
        /// </summary>
        private Boolean defaultRunOnceKey = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CIRegistry"/> class.
        /// </summary>
        public CIRegistry()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CIRegistry"/> class.
        /// </summary>
        /// <param name="eventLog">EventLog where all errors should be logged</param>
        public CIRegistry(EventLog eventLog)
        {
            this.eventLog = eventLog;
        }

        /// <summary>
        /// Gets the address of the user-data file from the registry.
        /// </summary>
        /// <returns>Address of the user-data file</returns>
        public String ReadUserDataFileKey()
        {
            // Get the key for UserDataFile
            return ReadSettingKey<String>("UserDataFile", this.defaultUserDataFile);
        }

        /// <summary>
        /// Gets the location of the log file from the registry.
        /// </summary>
        /// <returns>Location of the log file</returns>
        public String ReadLogFileKey()
        {
            // Get the key for LogFile
            return ReadSettingKey<String>("LogFile", this.defaultLogFile);
        }

        /// <summary>
        /// Gets the address of the Rijndael key from the registry.
        /// </summary>
        /// <returns>Address of the Rijndael key</returns>
        public String ReadSecureKey()
        {
            // Get the key for SecureKey
            return ReadSettingKey<String>("SecureKey", this.defaultSecureKey);
        }

        public Boolean ReadRunOnceKey()
        {
            // Get the key for RunOnce
            return ReadSettingKey<Boolean>("RunOnce", this.defaultRunOnceKey);
        }

        /// <summary>
        /// Gets the user data timeout key from the registry
        /// </summary>
        /// <returns>Returns the timeout in ms</returns>
        public Int32 ReadUserDataTimeout()
        {
            // Get the key for UserDataTimeout
            return ReadSettingKey<Int32>("UserDataTimeout", this.defaultUserDataTimeout);
        }

        /// <summary>
        /// Returns true if we should expect the user data to be base 64 encoded
        /// </summary>
        /// <returns></returns>
        public Boolean IsBase64Encoded()
        {
            return ReadSettingKey<Boolean>("IsBase64Encoded", this.defaultIsBase64Encoded);
        }

        /// <summary>
        /// Gets the registry setting for this service
        /// </summary>
        /// <param name="name">Name of the key to read</param>
        /// <param name="defaultValue">Default value to return if nothing is found in the registry</param>
        /// <returns>Value of the key</returns>
        private T ReadSettingKey<T>(String name, T defaultValue)
        {
            try
            {
                // Get the setting key
                RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                RegistryKey subKey = baseKey.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\CloudInit\\Settings", RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                // Return the setting key, if no value is found return the default
                return (T)Convert.ChangeType(subKey.GetValue(name, defaultValue), typeof(T), CultureInfo.CurrentCulture);
            }
            catch (SecurityException ex)
            {
                LogEvent(ex);
                return defaultValue;
            }
            catch (UnauthorizedAccessException ex)
            {
                LogEvent(ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Writes an entry to the event log if an event log object has been initialized
        /// </summary>
        /// <param name="ex">The exception to log</param>
        private void LogEvent(Exception ex)
        {
            if (this.eventLog != null && !String.IsNullOrEmpty(this.eventLog.Source))
                this.eventLog.WriteEntry(ex.StackTrace, EventLogEntryType.Error);
        }
    }
}
