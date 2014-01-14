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

using System.Text;

namespace ASC.FederatedLogin.Helpers
{
    public class HashHelper
    {
        public static int CombineHashCodes(int hash1, int hash2)
        {
            if (hash2 == 0)
                return hash1;
            return (((hash1 << 5) + hash1) ^ hash2);
        }

        //Use this luke!!!
        public static int StringHash(string text)
        {
            return text.GetHashCode();
        }

        public static string MD5(string text)
        {
            return MD5(text, Encoding.Default);
        }

        public static string MD5(string text, Encoding encoding)
        {
            return MD5String(encoding.GetBytes(text));
        }

        public static string MD5String(byte[] data)
        {
            byte[] hash = MD5(data);
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static byte[] MD5(byte[] data)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(data);
        }
    }
}