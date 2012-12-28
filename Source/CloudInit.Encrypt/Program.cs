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
using CloudInit.Registry;

namespace CloudInit
{
    /// <summary>
    /// Small console app to encrypt files
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Mains the specified args.
        /// </summary>
        /// <param name="args">Arg[0] - InputFile Arg[1] - OutputFile</param>
        static void Main(String[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please enter input file to encrypt and the output file");
            }

            String input = args[0];
            String output = args[0];

            if (args.Length >= 2)
                output = args[1];

            EncryptFile(input, output);
        }

        /// <summary>
        /// Encrypts the file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        private static void EncryptFile(String inputFile, String outputFile)
        {
            CIRegistry registry = new CIRegistry();
          
            RijndaelManaged crypto = new RijndaelManaged();
            Rfc2898DeriveBytes derivedBytes = new Rfc2898DeriveBytes(registry.ReadSecureKey(), new Byte[] { 90, 32, 230, 20, 59, 78, 112, 183 });
            Byte[] key = derivedBytes.GetBytes(crypto.KeySize / 8);
            Byte[] iv = derivedBytes.GetBytes(crypto.BlockSize / 8);

            Byte[] buffer = null;
            using (FileStream inFile = new FileStream(inputFile, FileMode.Open))
            {
                buffer = new Byte[inFile.Length];
                inFile.Read(buffer, 0, buffer.Length);
            }

            using(FileStream outFile = new FileStream(outputFile, FileMode.Create))
            using(CryptoStream cs = new CryptoStream(outFile, crypto.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
