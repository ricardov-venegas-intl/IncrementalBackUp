using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace IncrementalBackup
{
    class FSHA256Hasher : IFileHasher
    {
        public string CalculateHash(FileStream fileStream)
        {
            SHA256 hasher = SHA256.Create();
            var hash = hasher.ComputeHash(fileStream);
            return ByteArrayToString(hash);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
