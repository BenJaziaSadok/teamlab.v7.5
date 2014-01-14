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

namespace ASC.Mail.Net
{
    /// <summary>
    /// This class provides well known TCP/UDP service ports.
    /// </summary>
    public class WellKnownPorts
    {
        #region Members

        /// <summary>
        /// DNS protocol.
        /// </summary>
        public static readonly int DNS = 53;

        /// <summary>
        /// FTP - control (command) port.
        /// </summary>
        public static readonly int FTP_Control = 21;

        /// <summary>
        /// FTP over SSL protocol.
        /// </summary>
        public static readonly int FTP_Control_SSL = 990;

        /// <summary>
        /// FTP - data port.
        /// </summary>
        public static readonly int FTP_Data = 20;

        /// <summary>
        /// HTTP protocol.
        /// </summary>
        public static readonly int HTTP = 80;

        /// <summary>
        /// HTTPS protocol.
        /// </summary>
        public static readonly int HTTPS = 443;

        /// <summary>
        /// IMAP4 protocol.
        /// </summary>
        public static readonly int IMAP4 = 143;

        /// <summary>
        /// IMAP4 over SSL protocol.
        /// </summary>
        public static readonly int IMAP4_SSL = 993;

        /// <summary>
        /// NNTP (Network News Transfer Protocol)  protocol.
        /// </summary>
        public static readonly int NNTP = 119;

        /// <summary>
        /// NTP (Network Time Protocol) protocol.
        /// </summary>
        public static readonly int NTP = 123;

        /// <summary>
        /// POP3 protocol.
        /// </summary>
        public static readonly int POP3 = 110;

        /// <summary>
        /// POP3 over SSL protocol.
        /// </summary>
        public static readonly int POP3_SSL = 995;

        /// <summary>
        /// SMTP protocol.
        /// </summary>
        public static readonly int SMTP = 25;

        /// <summary>
        /// SMTP over SSL protocol.
        /// </summary>
        public static readonly int SMTP_SSL = 465;

        #endregion
    }
}