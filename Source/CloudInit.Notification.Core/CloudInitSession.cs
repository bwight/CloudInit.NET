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
using System.Net;
using System.Net.Sockets;

namespace CloudInit.Notification.Core
{
    /// <summary>
    /// Information about the current CloudInit session
    /// </summary>
    public class CloudInitSession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudInitSession"/> class.
        /// </summary>
        /// <param name="logData">The data from the log file</param>
        public CloudInitSession(String logData)
        {
            this.Data = logData;
            this.Machine = Environment.MachineName;
            this.Address = GetNetworkAddress();
        }

        /// <summary>
        /// Gets the name of the server running CloudInit
        /// </summary>
        public String Machine { get; private set; }
        /// <summary>
        /// Gets the ip address of the server running CloudInit
        /// </summary>
        public String Address { get; private set; }
        /// <summary>
        /// Gets the data that was saved to the log file.
        /// </summary>
        public String Data { get; private set; }

        /// <summary>
        /// Gets the network address of the local machine
        /// </summary>
        /// <returns></returns>
        private String GetNetworkAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress address in ipEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    return address.ToString();
            }

            return String.Empty;
        }
    }
}
