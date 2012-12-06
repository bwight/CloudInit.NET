//-----------------------------------------------------------------------
// <copyright file="IInputModule.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics;

namespace CloudInit.Modules.Core
{
    /// <summary>
    /// CloudInit module interface
    /// </summary>
    public interface IInputModule
    {
        /// <summary>
        /// Event log object to log errors
        /// </summary>
        EventLog EventLog { get; set; }

        /// <summary>
        /// Log file where the output of the script should be written
        /// </summary>
        String LogFile { get; set; }

        /// <summary>
        /// Header of the file that will cause the engine to use this module
        /// </summary>
        String Header { get; }

        /// <summary>
        /// Executes the script
        /// </summary>
        /// <param name="input">Input file to execute</param>
        /// <returns>
        /// Output log of the execution
        /// </returns>
        String Execute(String input);
    }
}
