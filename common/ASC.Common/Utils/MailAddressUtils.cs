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

using System.Net.Mail;
using System.Text;

namespace ASC.Common.Utils
{
    public static class MailAddressUtils
    {
        public static MailAddress Create(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                var firstPos = address.IndexOf('"');
                var lastPos = address.LastIndexOf('"');
                if (firstPos != -1 && firstPos < lastPos && address.IndexOf('"', firstPos + 1, lastPos - firstPos - 1) != -1)
                {
                    address = new StringBuilder(address).Replace("\"", string.Empty, firstPos + 1, lastPos - firstPos - 1).ToString();
                }
            }
            return new MailAddress(address);
        }

        public static MailAddress Create(string address, string displayName)
        {
            if (!string.IsNullOrEmpty(displayName))
            {
                displayName = displayName.Replace("\"", string.Empty);
                if (125 < displayName.Length)
                {
                    displayName = displayName.Substring(0, 125);
                }
            }
            return Create(ToSmtpAddress(address, displayName));
        }

        public static string ToEncodedString(this MailAddress m)
        {
            return ToSmtpAddress(m.Address, MimeHeaderUtils.EncodeMime(m.DisplayName));
        }


        private static string ToSmtpAddress(string address, string displayName)
        {
            return string.Format("\"{0}\" <{1}>", displayName, address);
        }
    }
}
