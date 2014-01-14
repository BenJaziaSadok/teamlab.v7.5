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
    #region usings

    using System;

    #endregion

    /// <summary>
    /// Implements single value header field.
    /// Used by header fields like To:,From:,CSeq:, ... .
    /// </summary>
    public class SIP_SingleValueHF<T> : SIP_HeaderField where T : SIP_t_Value
    {
        #region Members

        private T m_pValue;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public override string Value
        {
            get { return ToStringValue(); }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Property Value value may not be null !");
                }

                Parse(new StringReader(value));
            }
        }

        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public T ValueX
        {
            get { return m_pValue; }

            set { m_pValue = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <param name="value">Header field value.</param>
        public SIP_SingleValueHF(string name, T value) : base(name, "")
        {
            m_pValue = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses single value from specified reader.
        /// </summary>
        /// <param name="reader">Reader what contains </param>
        public void Parse(StringReader reader)
        {
            m_pValue.Parse(reader);
        }

        /// <summary>
        /// Convert this to string value.
        /// </summary>
        /// <returns>Returns this as string value.</returns>
        public string ToStringValue()
        {
            return m_pValue.ToStringValue();
        }

        #endregion

        // FIX ME: Change base class Value property or this to new name
    }
}