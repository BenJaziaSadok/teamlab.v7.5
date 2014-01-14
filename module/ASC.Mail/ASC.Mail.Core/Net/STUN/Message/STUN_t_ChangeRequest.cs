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

namespace ASC.Mail.Net.STUN.Message
{
    /// <summary>
    /// This class implements STUN CHANGE-REQUEST attribute. Defined in RFC 3489 11.2.4.
    /// </summary>
    public class STUN_t_ChangeRequest
    {
        #region Members

        private bool m_ChangeIP = true;
        private bool m_ChangePort = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public STUN_t_ChangeRequest() {}

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="changeIP">Specifies if STUN server must send response to different IP than request was received.</param>
        /// <param name="changePort">Specifies if STUN server must send response to different port than request was received.</param>
        public STUN_t_ChangeRequest(bool changeIP, bool changePort)
        {
            m_ChangeIP = changeIP;
            m_ChangePort = changePort;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if STUN server must send response to different IP than request was received.
        /// </summary>
        public bool ChangeIP
        {
            get { return m_ChangeIP; }

            set { m_ChangeIP = value; }
        }

        /// <summary>
        /// Gets or sets if STUN server must send response to different port than request was received.
        /// </summary>
        public bool ChangePort
        {
            get { return m_ChangePort; }

            set { m_ChangePort = value; }
        }

        #endregion
    }
}