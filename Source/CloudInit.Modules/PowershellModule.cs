//-----------------------------------------------------------------------
// <copyright file="PowershellModule.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using CloudInit.Modules.Core;

namespace CloudInit.Modules
{
    /// <summary>
    /// Default powershell module
    /// </summary>
    public class PowershellModule : PowershellModuleBase
    {
        /// <summary>
        /// Header of the file that will cause the engine to use this module
        /// </summary>
        public override String Header
        {
            get
            {
                return "#!";
            }
        }
    }
}
