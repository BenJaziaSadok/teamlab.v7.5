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

namespace ASC.Mail.Net.Dns.Client
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// TXT record class.
    /// </summary>
    [Serializable]
    public class DNS_rr_TXT : DNS_rr_base
    {
        #region Members

        private readonly string m_Text = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets text.
        /// </summary>
        public string Text
        {
            get { return m_Text; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="ttl">TTL value.</param>
        public DNS_rr_TXT(string text, int ttl) : base(QTYPE.TXT, ttl)
        {
            m_Text = text;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses resource record from reply data.
        /// </summary>
        /// <param name="reply">DNS server reply data.</param>
        /// <param name="offset">Current offset in reply data.</param>
        /// <param name="rdLength">Resource record data length.</param>
        /// <param name="ttl">Time to live in seconds.</param>
        public static DNS_rr_TXT Parse(byte[] reply, ref int offset, int rdLength, int ttl)
        {
            // TXT RR

            string text = Dns_Client.ReadCharacterString(reply, ref offset);

            return new DNS_rr_TXT(text, ttl);
        }

        #endregion
    }
}