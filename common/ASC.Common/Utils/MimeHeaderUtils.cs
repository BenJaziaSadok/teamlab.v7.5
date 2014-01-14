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
using System.Linq;
using System.Text;

namespace ASC.Common.Utils
{
    public static class MimeHeaderUtils
    {
        public static string EncodeMime(string mimeHeaderValue)
        {
            return EncodeMime(mimeHeaderValue, Encoding.UTF8, false);
        }

        public static string EncodeMime(string mimeHeaderValue, Encoding charset, bool split)
        {
            if (MustEncode(mimeHeaderValue))
            {
                var result = new StringBuilder();
                var data = charset.GetBytes(mimeHeaderValue);
                var maxEncodedTextSize = split ? 75 - ("=?" + charset.WebName + "?" + "B"/*Base64 encode*/ + "?" + "?=").Length : int.MaxValue;

                result.Append("=?" + charset.WebName + "?B?");
                var stored = 0;
                var base64 = Convert.ToBase64String(data);
                for (var i = 0; i < base64.Length; i += 4)
                {
                    // Encoding buffer full, create new encoded-word.
                    if (stored + 4 > maxEncodedTextSize)
                    {
                        result.Append("?=\r\n =?" + charset.WebName + "?B?");
                        stored = 0;
                    }

                    result.Append(base64, i, 4);
                    stored += 4;
                }
                result.Append("?=");
                return result.ToString();
            }
            return mimeHeaderValue;
        }

        public static bool MustEncode(string text)
        {
            return !string.IsNullOrEmpty(text) && text.Any(c => c > 127);
        }
    }
}