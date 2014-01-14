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
    /// This method provides data for RTP send stream related events and methods.
    /// </summary>
    public class RTP_SendStreamEventArgs : EventArgs
    {
        #region Members

        private readonly RTP_SendStream m_pStream;

        #endregion

        #region Properties

        /// <summary>
        /// Gets RTP stream.
        /// </summary>
        public RTP_SendStream Stream
        {
            get { return m_pStream; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">RTP send stream.</param>
        public RTP_SendStreamEventArgs(RTP_SendStream stream)
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