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
