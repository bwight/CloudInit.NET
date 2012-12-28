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
using System.Diagnostics;
using System.IO;
using System.Net;
using CloudInit.Modules.Core;
using StructureMap;
using CloudInit.Registry;
using System.Text;

namespace CloudInit
{
    /// <summary>
    /// 
    /// </summary>
    public class CIEngine
    {
        /// <summary>
        /// List of headers found while scaning the assemblies
        /// </summary>
        public static IList<String> Headers = new List<String>();

        /// <summary>
        /// Event log where errors should be written
        /// </summary>
        private EventLog eventLog = null;

        /// <summary>
        /// Log file where output of the script should be written
        /// </summary>
        private String logFile = String.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CIEngine"/> class.
        /// </summary>
        public CIEngine()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CIEngine"/> class.
        /// </summary>
        /// <param name="eventLog">The event log.</param>
        /// <param name="logFile">The log file.</param>
        public CIEngine(EventLog eventLog, String logFile)
        {
            this.eventLog = eventLog;
            this.logFile = logFile;
        }

        /// <summary>
        /// Executes the specified script URI.
        /// </summary>
        /// <param name="scriptUri">The script URI.</param>
        public void Execute(String scriptUri)
        {
            // Get the contents of the file to execute
            String contents = DownloadFile(scriptUri);
            
            using(StringReader reader = new StringReader(contents))
            {
                String line = String.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    foreach (String header in CIEngine.Headers)
                    {
                        if (line.StartsWith(header))
                        {
                            // Get the singleton module for this header type & execute the script
                            var module = ObjectFactory.GetNamedInstance<IInputModule>(header);
                            module.Execute(contents);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Downloads a file from an http endpoint
        /// </summary>
        /// <param name="url">Location of the file to be downloaded</param>
        /// <returns>Contents of the file downloaded</returns>
        private String DownloadFile(String url)
        {
            // Grab the user data timeout from the registry
            CIRegistry registry = new CIRegistry(this.eventLog);
            Int32 timeout = registry.ReadUserDataTimeout();
            Boolean isBase64Encoded = registry.IsBase64Encoded();

            // Make a web request to the provided url
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = timeout;

            // Get the response from the request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                // Read the steam into a string
                String content = reader.ReadToEnd();
                
                reader.Close();
                response.Close();
                
                // Return the file contents
                if (isBase64Encoded)
                {
                    Byte[] data = Convert.FromBase64String(content);
                    return Encoding.UTF8.GetString(data);
                }
                else
                {
                    return content;
                }
            }
        }
    }
}
