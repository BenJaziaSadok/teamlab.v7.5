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

namespace ASC.Mail.Net.SIP.Message
{
    /// <summary>
    /// Represents SIP message header field.
    /// </summary>
    public class SIP_HeaderField
    {
        #region Members

        private readonly string m_Name = "";
        private bool m_IsMultiValue;
        private string m_Value = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets header field name.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public virtual string Value
        {
            get { return m_Value; }

            set { m_Value = value; }
        }

        /// <summary>
        /// Gets if header field is multi value header field.
        /// </summary>
        public bool IsMultiValue
        {
            get { return m_IsMultiValue; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <param name="value">Header field value.</param>
        internal SIP_HeaderField(string name, string value)
        {
            m_Name = name;
            m_Value = value;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Sets property IsMultiValue value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetMultiValue(bool value)
        {
            m_IsMultiValue = value;
        }

        #endregion
    }
}