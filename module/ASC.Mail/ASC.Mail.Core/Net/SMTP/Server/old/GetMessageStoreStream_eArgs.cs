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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LumiSoft.Net.SMTP.Server
{
    /// <summary>
    /// This class provides data for the GetMessageStoreStream event.
    /// </summary>
    public class GetMessageStoreStream_eArgs
    {
        private SMTP_Session m_pSession     = null;
        private Stream       m_pStoreStream = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Reference to calling SMTP sesssion.</param>
        public GetMessageStoreStream_eArgs(SMTP_Session session)
        {
            m_pSession = session;
            m_pStoreStream = new MemoryStream();
        }


        #region Properties Implementation

        /// <summary>
		/// Gets reference to smtp session.
		/// </summary>
		public SMTP_Session Session
		{
			get{ return m_pSession; }
		}

        /// <summary>
        /// Gets or sets Stream where to store incoming message. Storing starts from stream current position.
        /// </summary>
        public Stream StoreStream
        {
            get{ return m_pStoreStream; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Property StoreStream value can't be null !");
                }
 
                m_pStoreStream = value; 
            }
        }

        #endregion

    }
}
