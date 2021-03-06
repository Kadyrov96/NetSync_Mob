﻿using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace MobTest
{
    class Hasher
    {
        static string GetHash(byte[] byteInput)
        {
            HashAlgorithmProvider provider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash cryptoHash = provider.CreateHash();
            IBuffer buffer = byteInput.AsBuffer();
            cryptoHash.Append(buffer);
            IBuffer b_hash = cryptoHash.GetValueAndReset();
            return CryptographicBuffer.EncodeToHexString(b_hash);
        }

        public static string GetFileHash(string filePath)
        {
            FileStream fileReader = File.OpenRead(filePath);
            byte[] bytesOfFile = new byte[fileReader.Length];
            fileReader.Read(bytesOfFile, 0, (int)fileReader.Length);
            return GetHash(bytesOfFile);
        }
        public static string GetStringHash(string _string)
        {
            byte[] bytesOfString = Encoding.UTF8.GetBytes(_string);
            return GetHash(bytesOfString);
        }
    }
}
