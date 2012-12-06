using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CloudInit.Modules.Core;
using CloudInit.Registry;
using System.Security.Cryptography;
using System.IO;

namespace CloudInit.Modules
{
    public class PowershellEncryptedModule : PowershellModuleBase
    {
        /// <summary>
        /// Header of the file that will cause the engine to use this module
        /// </summary>
        public override String Header
        {
            get { return "#encrypted"; }
        }

        public override string Execute(string input)
        {
            return base.Execute(input);
        }

        protected virtual String GetEncryptedContent(Byte[] content)
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
