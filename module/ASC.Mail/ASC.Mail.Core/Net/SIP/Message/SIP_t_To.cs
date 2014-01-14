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
    using System.Text;

    #endregion

    /// <summary>
    /// Implements SIP "To" value. Defined in RFC 3261.
    /// The To header field specifies the logical recipient of the request.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3261 Syntax:
    ///     To        = ( name-addr / addr-spec ) *( SEMI to-param )
    ///     to-param  = tag-param / generic-param
    ///     tag-param = "tag" EQUAL token
    /// </code>
    /// </remarks>
    public class SIP_t_To : SIP_t_ValueWithParams
    {
        #region Members

        private readonly SIP_t_NameAddress m_pAddress;

        #endregion

        #region Properties

        /// <summary>
        /// Gets to address.
        /// </summary>
        public SIP_t_NameAddress Address
        {
            get { return m_pAddress; }
        }

        /// <summary>
        /// Gets or sets tag parameter value.
        /// The "tag" parameter serves as a general mechanism for dialog identification.
        /// Value null means that 'tag' paramter doesn't exist.
        /// </summary>
        public string Tag
        {
            get
            {
                SIP_Parameter parameter = Parameters["tag"];
                if (parameter != null)
                {
                    return parameter.Value;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Parameters.Remove("tag");
                }
                else
                {
                    Parameters.Set("tag", value);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">To: header field value.</param>
        public SIP_t_To(string value)
        {
            m_pAddress = new SIP_t_NameAddress();

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="address">To address.</param>
        public SIP_t_To(SIP_t_NameAddress address)
        {
            m_pAddress = address;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses "To" from specified value.
        /// </summary>
        /// <param name="value">SIP "To" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("reader");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "To" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /* To       =  ( name-addr / addr-spec ) *( SEMI to-param )
               to-param =  tag-param / generic-param
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Parse address
            m_pAddress.Parse(reader);

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "To" value.
        /// </summary>
        /// <returns>Returns "To" value.</returns>
        public override string ToStringValue()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append(m_pAddress.ToStringValue());
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }

        #endregion
    }
}