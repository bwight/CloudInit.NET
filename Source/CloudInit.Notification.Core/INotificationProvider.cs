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
