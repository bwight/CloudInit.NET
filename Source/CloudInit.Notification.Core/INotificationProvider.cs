//-----------------------------------------------------------------------
// <copyright file="INotificationProvider.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Microsoft.Win32;

namespace CloudInit.Notification.Core
{
    /// <summary>
    /// Interface that exposes required properties for a notification provider
    /// </summary>
    public interface INotificationProvider
    {
        /// <summary>
        /// Gets the settings path in the registry for this provider
        /// </summary>
        String SettingsPath { get; }
        /// <summary>
        /// Gets the name of the provider
        /// </summary>
        String ProviderName { get; }
        /// <summary>
        /// Gets a value indicating whether this provider is active
        /// </summary>
        Boolean IsActive { get; }

        /// <summary>
        /// Adds this provider to the registry
        /// </summary>
        void Add();
        /// <summary>
        /// Removes this provider from the registry
        /// </summary>
        void Remove();
        /// <summary>
        /// Sets the setting for this provider
        /// </summary>
        /// <param name="name">The registry key name</param>
        /// <param name="value">The desired key value</param>
        /// <param name="valueKind">Kind of the value</param>
        void SetSetting(String name, Object value, RegistryValueKind valueKind);
        /// <summary>
        /// Gets the setting for this provider
        /// </summary>
        /// <typeparam name="T">Type of object to return from registry</typeparam>
        /// <param name="name">The registry key name</param>
        /// <returns></returns>
        T GetSetting<T>(String name);
        /// <summary>
        /// Notifies the provider of a CloudInit log file
        /// </summary>
        /// <param name="session">The current CloudInit session</param>
        void Notify(CloudInitSession session);
    }
}
