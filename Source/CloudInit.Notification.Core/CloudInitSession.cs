//-----------------------------------------------------------------------
// <copyright file="INotificationProvider.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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
