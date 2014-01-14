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

namespace ASC.Mail.Autoreply.Utility
{
    public static class StringExtensions
    {
        public static string TrimStart(this string str, string trimStr)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            while (str.StartsWith(trimStr))
            {
                str = str.Remove(0, trimStr.Length);
            }

            return str;
        }

        public static string TrimEnd(this string str, string trimStr)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            while (str.EndsWith(trimStr))
            {
                str = str.Remove(str.Length - trimStr.Length);
            }

            return str;
        }

        public static string Trim(this string str, string trimStr)
        {
            return str.TrimStart(trimStr).TrimEnd(trimStr);
        }
    }
}
