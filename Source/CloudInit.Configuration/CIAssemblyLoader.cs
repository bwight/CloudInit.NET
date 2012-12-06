﻿//-----------------------------------------------------------------------
// <copyright file="AssemblyLoader.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace CloudInit.Configuration
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
        public static void Configure(String path)
        {
            var types = new List<Type>();

            // Get a list of all the types in the assemblies found in the base directory
            var assemblies = GetAssembliesInDirectoryWithExtension(path);
            Array.ForEach(assemblies.ToArray(), a => { foreach (Type t in a.GetTypes()) types.Add(t); });

            TypesToScan = types;
        }

        /// <summary>
        /// Scans assemblies for the provided type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> ScanFor<T>()
        {
            foreach (var type in CIAssemblyLoader.TypesToScan)
            {
                // Make sure the type is assignable and make sure it does not inherit IConfigureThisEndpoint
                if (typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    yield return type;
                }
            }
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