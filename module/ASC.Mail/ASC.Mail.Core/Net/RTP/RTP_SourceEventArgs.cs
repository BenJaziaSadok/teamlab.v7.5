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
    /// This class provides data for RTP source related evetns.
    /// </summary>
    public class RTP_SourceEventArgs : EventArgs
    {
        #region Members

        private readonly RTP_Source m_pSource;

        #endregion

        #region Properties

        /// <summary>
        /// Gets RTP source.
        /// </summary>
        public RTP_Source Source
        {
            get { return m_pSource; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="source">RTP source.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>source</b> is null reference.</exception>
        public RTP_SourceEventArgs(RTP_Source source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            m_pSource = source;
        }

        #endregion
    }
}