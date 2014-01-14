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

namespace ASC.Mail.Net.Mail
{
    #region usings

    using System;
    using System.Text;
    using MIME;

    #endregion

    /// <summary>
    /// This class provides mail message related utility methods.
    /// </summary>
    public class Mail_Utils
    {
        #region Internal methods

        /// <summary>
        /// Reads SMTP "Mailbox" from the specified MIME reader.
        /// </summary>
        /// <param name="reader">MIME reader.</param>
        /// <returns>Returns SMTP "Mailbox" or null if no SMTP mailbox available.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>reader</b> is null reference.</exception>
        internal static string SMTP_Mailbox(MIME_Reader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // TODO:

            /* RFC 5321.
                Mailbox        = Local-part "@" ( Domain / address-literal )
                Local-part     = Dot-string / Quoted-string ; MAY be case-sensitive
                Dot-string     = Atom *("."  Atom)
            */

            StringBuilder retVal = new StringBuilder();
            if (reader.Peek(true) == '\"')
            {
                retVal.Append("\"" + reader.QuotedString() + "\"");
            }
            else
            {
                retVal.Append(reader.DotAtom());
            }

            if (reader.Peek(true) != '@')
            {
                return null; ;
            }
            else
            {
                // Eat "@".
                reader.Char(true);

                retVal.Append('@');
                retVal.Append(reader.DotAtom());
            }

            return retVal.ToString();
        }

        #endregion
    }
}