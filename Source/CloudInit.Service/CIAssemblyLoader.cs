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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace CloudInit
{
    // #################################################
    // Code taken from parts of NServiceBus
    // http://nservicebus.com
    // #################################################

    /// <summary>
    /// Loads all the assemblies and types for scaning
    /// </summary>
    public class CIAssemblyLoader
    {
        /// <summary>
        /// Returns types in assemblies found in the current directory.
        /// </summary>
        public static IEnumerable<Type> TypesToScan { get; private set; }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        public static void Configure()
        {
            var types = new List<Type>();

            // Get a list of all the types in the assemblies found in the base directory
            var assemblies = GetAssembliesInDirectoryWithExtension(AppDomain.CurrentDomain.BaseDirectory);
            Array.ForEach(assemblies.ToArray(), a => { foreach (Type t in a.GetTypes()) types.Add(t); });

            TypesToScan = types;
        }

        /// <summary>
        /// Load and return all assemblies in the given directory except the given ones to exclude
        /// </summary>
        /// <param name="path">The path of all the assemblies</param>
        /// <returns>A list of assemblies in the provided directory</returns>
        private static IEnumerable<Assembly> GetAssembliesInDirectoryWithExtension(String path)
        {
            foreach (FileInfo file in new DirectoryInfo(path).GetFiles("*.dll", SearchOption.AllDirectories))
                yield return Assembly.LoadFrom(file.FullName);
            foreach (FileInfo file in new DirectoryInfo(path).GetFiles("*.exe", SearchOption.AllDirectories))
                yield return Assembly.LoadFrom(file.FullName);
        }
    }
}
