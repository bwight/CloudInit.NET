//-----------------------------------------------------------------------
// <copyright file="NotificationProviderBase.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace CloudInit.Notification.Core
{
    /// <summary>
    /// Base class for a notification provider
    /// </summary>
    public abstract class NotificationProviderBase : INotificationProvider
    {
        /// <summary>
        /// Gets the name of the provider
        /// </summary>
        public abstract String  ProviderName 
        { 
            get; 
        }
        /// <summary>
        /// Gets the settings path in the registry for this provider
        /// </summary>
        public String SettingsPath
        {
            get
            {
                return String.Format("SYSTEM\\CurrentControlSet\\services\\CloudInit\\Settings\\{0}", GenerateGuid());
            }
        }
        /// <summary>
        /// Gets a value indicating whether this provider is active
        /// </summary>
        public Boolean IsActive
        {
            get
            {
                return GetSetting<Boolean>("IsActive");
            }
        }

        /// <summary>
        /// Adds this provider to the registry
        /// </summary>
        public virtual void Add() 
        {
            Registry.LocalMachine.CreateSubKey(SettingsPath);
            SetSetting("IsActive", 1, RegistryValueKind.DWord);

        }
        /// <summary>
        /// Removes this provider from the registry
        /// </summary>
        public virtual void Remove() 
        {
            Registry.LocalMachine.DeleteSubKeyTree(SettingsPath);
        }
        /// <summary>
        /// Sets the setting for this provider
        /// </summary>
        /// <param name="name">The registry key name</param>
        /// <param name="value">The desired key value</param>
        /// <param name="valueKind">Kind of the value</param>
        public void SetSetting(String name, Object value, RegistryValueKind valueKind)
        {
            RegistryKey subKey = Registry.LocalMachine.OpenSubKey(SettingsPath, true);
            subKey.SetValue(name, value, valueKind);
        }
        /// <summary>
        /// Gets the setting for this provider
        /// </summary>
        /// <typeparam name="T">Type of object to return from registry</typeparam>
        /// <param name="name">The registry key name</param>
        /// <returns></returns>
        public T GetSetting<T>(String name)
        {
            try
            {
                RegistryKey subKey = Registry.LocalMachine.OpenSubKey(SettingsPath);
                return (T)Convert.ChangeType(subKey.GetValue(name), typeof(T), CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Notifies the provider of a CloudInit log file
        /// </summary>
        /// <param name="session">The current CloudInit session</param>
        public abstract void Notify(CloudInitSession session);

        /// <summary>
        /// Generates a deterministic GUID.
        /// </summary>
        /// <returns></returns>
        protected Guid GenerateGuid()
        {
            //use MD5 hash to get a hash of the string:
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(this.ProviderName);
            byte[] hashBytes = provider.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
           
    }
}
