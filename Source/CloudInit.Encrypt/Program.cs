//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="SBR Net Marketing LLC" author="Brian Wight">
//     Copyright 2011 SBR Net Marketing LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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
