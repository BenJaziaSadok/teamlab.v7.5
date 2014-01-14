/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ASC.Core
{
    class Crypto
    {
        static byte[] GetSK1(bool rewrite) 
        {
            return GetSK(rewrite.GetType().Name.Length);
        }

        static byte[] GetSK2(bool rewrite)
        {
            return GetSK(rewrite.GetType().Name.Length*2);
        }

        static byte[] GetSK(int seed) {
            var random = new Random(seed);
            var randomKey = new byte[32];
            for (int i = 0; i < randomKey.Length; i++)
            {
                randomKey[i] = (byte)random.Next(byte.MaxValue);
            }
            return randomKey;
        }

        internal static string GetV(string data, int keyno, bool reverse)
        {
            var hasher = Rijndael.Create();
            hasher.Key = keyno == 1 ? GetSK1(false) : GetSK2(false);
            hasher.IV = new byte[hasher.BlockSize >> 3];

            string result = null;
            if (reverse)
            {
                using (var ms = new MemoryStream())
                using (var ss = new CryptoStream(ms, hasher.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var buffer = Encoding.Unicode.GetBytes(data);
                    ss.Write(buffer, 0, buffer.Length);
                    ss.FlushFinalBlock();
                    hasher.Clear();
                    result = Convert.ToBase64String(ms.ToArray());
                }
            }
            else
            {
                var bytes = Convert.FromBase64String(data);
                using (var ms = new MemoryStream(bytes))
                using (var ss = new CryptoStream(ms, hasher.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    var buffer = new byte[bytes.Length];
                    int size = ss.Read(buffer, 0, buffer.Length);
                    hasher.Clear();
                    var newBuffer = new byte[size];
                    Array.Copy(buffer, newBuffer, size);
                    result = Encoding.Unicode.GetString(newBuffer);
                }
            }

            return result;
        }

        internal static byte[] GetV(byte[] data, int keyno, bool reverse)
        {
            var hasher = Rijndael.Create();
            hasher.Key = keyno == 1 ? GetSK1(false) : GetSK2(false);
            hasher.IV = new byte[hasher.BlockSize >> 3];

            byte[] result = null;
            if (reverse)
            {
                using (var ms = new MemoryStream())
                using (var ss = new CryptoStream(ms, hasher.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    var buffer = data;
                    ss.Write(buffer, 0, buffer.Length);
                    ss.FlushFinalBlock();
                    hasher.Clear();
                    result = ms.ToArray();
                }
            }
            else
            {
                var bytes = data;
                using (var ms = new MemoryStream(bytes))
                using (var ss = new CryptoStream(ms, hasher.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    var buffer = new byte[bytes.Length];
                    int size = ss.Read(buffer, 0, buffer.Length);
                    hasher.Clear();
                    var newBuffer = new byte[size];
                    Array.Copy(buffer, newBuffer, size);
                    result = newBuffer;
                }
            }

            return result;
        }

        public static string GeneratePassword(int length)
        {
            var noise = "1234567890mnbasdflkjqwerpoiqweyuvcxnzhdkqpsdk";
            var random = new Random();
            var pwd = string.Empty;
            while (0 < length--) pwd += noise[random.Next(noise.Length)];
            return pwd;
        }
    }
}
