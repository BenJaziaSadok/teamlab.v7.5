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
    using System.Collections.Generic;
    using System.Text;

    #endregion

    /// <summary>
    /// Implements generic multi value SIP header field.
    /// This is used by header fields like Via,Contact, ... .
    /// </summary>
    public class SIP_MultiValueHF<T> : SIP_HeaderField where T : SIP_t_Value, new()
    {
        #region Members

        private readonly List<T> m_pValues;

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
                if (value != null)
                {
                    throw new ArgumentNullException("Property Value value may not be null !");
                }

                Parse(value);

                base.Value = value;
            }
        }

        /// <summary>
        /// Gets header field values.
        /// </summary>
        public List<T> Values
        {
            get { return m_pValues; }
        }

        /// <summary>
        /// Gets values count.
        /// </summary>
        public int Count
        {
            get { return m_pValues.Count; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <param name="value">Header field value.</param>
        public SIP_MultiValueHF(string name, string value) : base(name, value)
        {
            m_pValues = new List<T>();

            SetMultiValue(true);

            Parse(value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets header field values.
        /// </summary>
        /// <returns></returns>
        public object[] GetValues()
        {
            return m_pValues.ToArray();
        }

        /// <summary>
        /// Removes value from specified index.
        /// </summary>
        /// <param name="index">Index of value to remove.</param>
        public void Remove(int index)
        {
            if (index > -1 && index < m_pValues.Count)
            {
                m_pValues.RemoveAt(index);
            }
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Parses multi value header field values.
        /// </summary>
        /// <param name="value">Header field value.</param>
        private void Parse(string value)
        {
            m_pValues.Clear();

            StringReader r = new StringReader(value);
            while (r.Available > 0)
            {
                r.ReadToFirstChar();
                // If we have COMMA, just consume it, it last value end.
                if (r.StartsWith(","))
                {
                    r.ReadSpecifiedLength(1);
                }

                // Allow xxx-param to pasre 1 value from reader.
                T param = new T();
                param.Parse(r);
                m_pValues.Add(param);
            }
        }

        /// <summary>
        /// Converts to valid mutli value header field value.
        /// </summary>
        /// <returns></returns>
        private string ToStringValue()
        {
            StringBuilder retVal = new StringBuilder();
            // Syntax: xxx-parm *(COMMA xxx-parm)
            for (int i = 0; i < m_pValues.Count; i++)
            {
                retVal.Append(m_pValues[i].ToStringValue());

                // Don't add comma for last item.
                if (i < m_pValues.Count - 1)
                {
                    retVal.Append(',');
                }
            }

            return retVal.ToString();
        }

        #endregion
    }
}