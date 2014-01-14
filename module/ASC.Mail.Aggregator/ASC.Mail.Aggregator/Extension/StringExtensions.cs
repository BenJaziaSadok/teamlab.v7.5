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

namespace ASC.Mail.Aggregator.Extension
{
    public static class StringExtensions
    {
        public static string GetMD5(this string text)
        {
            var x = 
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(text);
            bs = x.ComputeHash(bs);
            var s = new StringBuilder(32);
            foreach (var b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static string Prefix(this string str, string prefix)
        {
            return str.Prefix(prefix, ".");
        }

        public static string Prefix(this string str, string prefix, string separator)
        {
            return prefix + separator + str;
        }
    }
}
