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

namespace ASC.Mail.Net.MIME
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// Represents MIME header field parameter.
    /// </summary>
    public class MIME_h_Parameter
    {
        #region Members

        private readonly string m_Name = "";
        private bool m_IsModified;
        private string m_Value = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets if this header field parameter is modified since it has loaded.
        /// </summary>
        /// <remarks>All new added header fields parameters has <b>IsModified = true</b>.</remarks>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public bool IsModified
        {
            get { return m_IsModified; }
        }

        /// <summary>
        /// Gets parameter name.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets or sets parameter value. Value null means not specified.
        /// </summary>
        public string Value
        {
            get { return m_Value; }

            set
            {
                m_Value = value;
                m_IsModified = true;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value. Value null means not specified.</param>
        public MIME_h_Parameter(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            m_Name = name;
            m_Value = value;
        }

        #endregion
    }
}