//-----------------------------------------------------------------------
// <copyright file="NewCISecretKeyCommand.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Text;

namespace CloudInit.Configuration
{
    /// <summary>
    /// This command generates a new key for the CloudInit security
    /// <example>
    /// New-CISecretKey -Size 32
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "CISecretKey")]
    public class NewCISecretKeyCommand : Cmdlet
    {
        private Int32 size = 32;

        /// <summary>
        /// Sets the desired size of the secret key.
        /// </summary>
        [Parameter(HelpMessage="The desired size in bytes of the secret key",Mandatory=false, Position=0)]
        public Int32 Size { set { this.size = value; } }

        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            try
            {
                // Create a new set of random bytes
                RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider();
                Byte[] data = new Byte[this.size];
                rngProvider.GetBytes(data);

                // Convert the bytes into a readable key
                String secretKey = ConvertBytes(data);

                WriteObject(secretKey);                
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, String.Empty, ErrorCategory.InvalidOperation, this));
            }
        }

        /// <summary>
        /// Converts the bytes into a readable string
        /// </summary>
        /// <param name="bytes">The bytes to be converted</param>
        /// <returns>Readable string</returns>
        private String ConvertBytes(Byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Byte b in bytes)
                sb.Append(Convert.ToChar((Int32)b % 63 + 59));

            // Replace the special characters
            return sb.ToString()
                .Replace(';', '1').Replace('<', '2').Replace('=', '3')
                .Replace('>', '4').Replace('?', '5').Replace('@', '6')
                .Replace('[', '7').Replace('\\', '8').Replace(']', '9')
                .Replace('^', '-').Replace('`', '0');
        }
    }
}
