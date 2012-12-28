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
