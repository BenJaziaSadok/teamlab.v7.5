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

namespace ASC.Mail.Net.IMAP.Client
{
    /// <summary>
    /// IMAP namespace. Defined in RFC 2342.
    /// </summary>
    public class IMAP_Namespace
    {
        #region Members

        private readonly string m_Delimiter = "";
        private readonly string m_Name = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets namespace name.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets namespace hierarchy delimiter.
        /// </summary>
        public string Delimiter
        {
            get { return m_Delimiter; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Namespace name.</param>
        /// <param name="delimiter">Namespace hierarchy delimiter.</param>
        public IMAP_Namespace(string name, string delimiter)
        {
            m_Name = name;
            m_Delimiter = delimiter;
        }

        #endregion
    }
}