//-----------------------------------------------------------------------
// <copyright file="PowershellIncludeEncryptedModule.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CloudInit.Registry;

namespace CloudInit.Modules
{
    /// <summary>
    /// Reads an encrypted powershell script and executes the contents
    /// </summary>
    public class PowershellIncludeEncryptedModule : PowershellIncludeModule
    {
        /// <summary>
        /// Header of the file that will cause the engine to use this module
        /// </summary>
        public override String Header
        {
            get { return "#encrypted include"; }
        }

        /// <summary>
        /// Reads the contents of the file.
        /// </summary>
        /// <param name="content">The content of the file downloaded</param>
        /// <returns>Decrypted content of the file</returns>
        protected override String ReadContent(Byte[] content)
        {
            CIRegistry registry = new CIRegistry();
            String secureKey = registry.ReadSecureKey();

            RijndaelManaged crypto = new RijndaelManaged();
            Rfc2898DeriveBytes derivedBytes = new Rfc2898DeriveBytes(registry.ReadSecureKey(), new Byte[] { 90, 32, 230, 20, 59, 78, 112, 183 });
            Byte[] key = derivedBytes.GetBytes(crypto.KeySize / 8);
            Byte[] iv = derivedBytes.GetBytes(crypto.BlockSize / 8);

            using(MemoryStream ms = new MemoryStream(content))
            using(CryptoStream cs = new CryptoStream(ms, crypto.CreateDecryptor(key, iv), CryptoStreamMode.Read))
            using (MemoryStream msOut = new MemoryStream())
            {
                Int32 buffer;
                while ((buffer = cs.ReadByte()) != -1)
                    msOut.WriteByte((Byte)buffer);

                msOut.Flush();
                return Encoding.Default.GetString(msOut.ToArray());
            }
        }
    }
}
