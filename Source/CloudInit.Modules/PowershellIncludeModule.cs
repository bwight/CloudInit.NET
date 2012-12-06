//-----------------------------------------------------------------------
// <copyright file="PowershellIncludeModule.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using CloudInit.Modules.Core;

namespace CloudInit.Modules
{
    /// <summary>
    /// Powershell module that downloads a list of scripts and executes each one in order
    /// </summary>
    public class PowershellIncludeModule : PowershellModuleBase
    {
        /// <summary>
        /// Header of the file that will cause the engine to use this module
        /// </summary>
        public override String Header
        {
            get
            {
                return "#include";
            }
        }

        /// <summary>
        /// Executes the script
        /// </summary>
        /// <param name="input">Input file to execute</param>
        /// <returns>
        /// Output log of the execution
        /// </returns>
        public override String Execute(String input)
        {
            using (StringWriter writer = new StringWriter())
            using (StringReader reader = new StringReader(input))
            {
                String line = String.Empty;

                // Read all the lines of the file ignoring any that are comments
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("#"))
                    {
                        // Download the file and execute contents as a powershell script
                        String data = ReadContent(DownloadFile(line));
                        String output = base.Execute(data);
                        writer.WriteLine(output);
                    }
                }
                return writer.ToString();
            }
        }

        /// <summary>
        /// Reads the contents of the file.
        /// </summary>
        /// <param name="content">The content of the file downloaded</param>
        /// <returns></returns>
        protected virtual String ReadContent(Byte[] content)
        {
            return Encoding.ASCII.GetString(content);
        }

        /// <summary>
        /// Downloads a file from an http endpoint
        /// </summary>
        /// <param name="url">Location of the file to be downloaded</param>
        /// <returns>Contents of the file downloaded</returns>
        protected virtual Byte[] DownloadFile(String url)
        {
            // Make a web request to the provided url
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 30000;

            // Allow the request to handle gzip or deflate protocols
            request.Headers["accept-encoding"] = "gzip,deflate";

            // Create response stream for decoding gzip and deflate
            Stream responseStream = null;
            
            // Get the response from the request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    // Read the steam into a bytearray
                    responseStream = response.GetResponseStream();

                    // If the content is compressed using gzip, use the gzip stream to read the compressed stream 
                    if (response.ContentEncoding.ToLower() == "gzip")
                    {
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        this.WriteOutput(String.Format("Decompressed gziped file: {0}", url));
                    }

                    // If the content is compressed using deflate, use the deflate stream to read the compressed stream
                    if (response.ContentEncoding.ToLower() == "deflate")
                    {
                        responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                        this.WriteOutput(String.Format("Decompressed deflated file: {0}", url));
                    }

                    // Copy stream to memory stream
                    responseStream.CopyTo(ms);

                    this.WriteOutput(String.Format("Downloaded file: {0}", url));

                    // Return the file contents
                    return ms.ToArray();
                }
                finally
                {
                    if(responseStream != null)
                        responseStream.Close();
                    
                    if(response != null)
                        response.Close();
                }
            }
        }
    }
}
