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

namespace ASC.Mail.Net.RTP
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This method provides data for RTP receive stream related events and methods.
    /// </summary>
    public class RTP_ReceiveStreamEventArgs : EventArgs
    {
        #region Members

        private readonly RTP_ReceiveStream m_pStream;

        #endregion

        #region Properties

        /// <summary>
        /// Gets RTP stream.
        /// </summary>
        public RTP_ReceiveStream Stream
        {
            get { return m_pStream; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">RTP stream.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        public RTP_ReceiveStreamEventArgs(RTP_ReceiveStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            m_pStream = stream;
        }

        #endregion
    }
}