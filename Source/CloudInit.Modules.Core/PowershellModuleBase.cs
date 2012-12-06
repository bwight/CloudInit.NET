//-----------------------------------------------------------------------
// <copyright file="PowershellModuleBase.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace CloudInit.Modules.Core
{
    /// <summary>
    /// Base class to execute a powershell script
    /// </summary>
    public abstract class PowershellModuleBase : IInputModule
    {
        /// <summary>
        /// Event log object to log errors
        /// </summary>
        public EventLog EventLog { get; set; }

        /// <summary>
        /// Log file where the output of the script should be written
        /// </summary>
        public String LogFile { get; set; }

        /// <summary>
        /// Header of the file that will cause the engine to use this module
        /// </summary>
        public abstract String Header { get; }

        /// <summary>
        /// Executes the script
        /// </summary>
        /// <param name="input">Input file to execute</param>
        /// <returns>
        /// Output log of the execution
        /// </returns>
        public virtual String Execute(String input)
        {
            try
            {
                // ########################################################
                // http://www.codeproject.com/KB/cs/HowToRunPowerShell.aspx
                // ########################################################

                // Create Powershell Runspace
                Runspace runspace = RunspaceFactory.CreateRunspace();
                runspace.Open();

                // Create a pipeline and feed it the script text
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(input);
                pipeline.Commands.Add("Out-String");

                // Execute the script
                Collection<PSObject> results = pipeline.Invoke();

                // Close the runspace
                runspace.Close();

                // Convert the script result into a single string
                using (StringWriter writer = new StringWriter())
                {
                    foreach (PSObject obj in results)
                    {
                        String output = obj.ToString();
                        WriteOutput(output);
                        writer.WriteLine(output);
                    }
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                LogEvent(ex);
                WriteOutput(String.Format("ERROR: {0}" ,ex.Message));
                return String.Empty;
            }
        }

        /// <summary>
        /// Writes an entry to the event log if an event log object has been initialized
        /// </summary>
        /// <param name="ex">The exception to log</param>
        protected void LogEvent(Exception ex)
        {
            if (this.EventLog != null && !String.IsNullOrEmpty(this.EventLog.Source))
                this.EventLog.WriteEntry(ex.StackTrace, EventLogEntryType.Error);
        }

        /// <summary>
        /// Writes the output to the log file
        /// </summary>
        /// <param name="output">The output.</param>
        protected void WriteOutput(String output)
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
