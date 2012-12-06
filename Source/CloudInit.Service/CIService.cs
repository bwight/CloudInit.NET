//-----------------------------------------------------------------------
// <copyright file="CIService.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using CloudInit.Modules.Core;
using CloudInit.Registry;
using StructureMap;
using CloudInit.Notification.Core;

namespace CloudInit
{
    /// <summary>
    /// Bootstaping service for CloudInit
    /// </summary>
    public class CIService
    {
        /// <summary>
        /// Log file for startup output
        /// </summary>
        protected static String LogFile = String.Empty;

        /// <summary>
        /// Executes the main code for this service
        /// </summary>
        /// <param name="state">The service object</param>
        public static void Main(Object state)
        {
            // Set the event log from the servicebase
            EventLog EventLog = state == null ? new EventLog("Application") : ((ServiceBase)state).EventLog;

            // Load registry settings
            CIRegistry registry = new CIRegistry(EventLog);
            var address = registry.ReadUserDataFileKey();
            var logFile = registry.ReadLogFileKey();

            LogFile = logFile;

            WriteOutput("Starting Service");

            // Load all the assemblies and scan the types
            CIAssemblyLoader.Configure();

            ObjectFactory.Configure(c =>
            {
                foreach (var type in CIAssemblyLoader.TypesToScan)
                {
                    // Make sure the type is assignable to IInputModule
                    if (typeof(IInputModule).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        // Run the init method to configure this database endpoint
                        IInputModule obj = Activator.CreateInstance(type) as IInputModule;
                        CIEngine.Headers.Add(obj.Header);

                        // Set the properties for this object
                        obj.EventLog = EventLog;
                        obj.LogFile = logFile;

                        // Add the object to the ObjectFactory
                        c.For<IInputModule>().Singleton().Use(obj).Named(obj.Header);

                        WriteOutput(String.Format("Adding module: {0}", type.ToString()));
                    }
                }
            });

            try
            {
                // Execute the script
                CIEngine engine = new CIEngine(EventLog, logFile);
                engine.Execute(address);
            }
            catch (Exception ex)
            {
                WriteOutput(ex.Message);
                WriteOutput(ex.StackTrace);
            }

            // Get the log file
            FileInfo info = new FileInfo(logFile);

            foreach (var type in CIAssemblyLoader.TypesToScan)
            {
                // Make sure the type is assignable to INotificationProvider
                if (typeof(INotificationProvider).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    INotificationProvider obj = Activator.CreateInstance(type) as INotificationProvider;

                    // Make sure the notification is active before sending the notify command
                    if (obj.IsActive)
                    {
                        using(FileStream stream = info.OpenRead())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            // Send the log file via this notification provider
                            CloudInitSession session = new CloudInitSession(reader.ReadToEnd());
                            obj.Notify(session);
                        }
                    }
                }
            }

            if (state != null)
            {
                ServiceBase service = (ServiceBase)state;

                // Disable this service if run once is enabled
                if (registry.ReadRunOnceKey())
                {
                    ServiceController sc = new ServiceController(service.ServiceName);
                    ServiceHelper.ChangeStartMode(sc, ServiceStartMode.Disabled);
                }

                // Stop the service
                service.Stop();
            }
        }


        /// <summary>
        /// Writes the output to the log file
        /// </summary>
        /// <param name="output">The output.</param>
        protected static void WriteOutput(String output)
        {
            FileInfo file = new FileInfo(LogFile);

            if (!file.Exists)
            {
                if (!file.Directory.Exists)
                    file.Directory.Create();
            }

            using (FileStream fs = file.Open(FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(String.Format("[ {0:yyyy-MM-dd HH:mm:ss} ] {1}", DateTime.UtcNow, output.Trim()));
            }

        }
    }
}
