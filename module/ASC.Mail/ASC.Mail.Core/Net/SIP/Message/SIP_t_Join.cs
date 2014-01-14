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
    /// Implements SIP "Join" value. Defined in RFC 3911.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3911 Syntax:
    ///     Join       = callid *(SEMI join-param)
    ///     join-param = to-tag / from-tag / generic-param
    ///     to-tag     = "to-tag" EQUAL token
    ///     from-tag   = "from-tag" EQUAL token
    /// </code>
    /// </remarks>
    public class SIP_t_Join : SIP_t_ValueWithParams
    {
        #region Members

        private SIP_t_CallID m_pCallID;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets call ID value.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised �when null value passed.</exception>
        public SIP_t_CallID CallID
        {
            get { return m_pCallID; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("CallID");
                }

                m_pCallID = value;
            }
        }

        /// <summary>
        /// Gets or sets to-tag parameter value. This value is mandatory.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid ToTag value is passed.</exception>
        public string ToTag
        {
            get
            {
                SIP_Parameter parameter = Parameters["to-tag"];
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
                    throw new ArgumentException("ToTag is mandatory and cant be null or empty !");
                }
                else
                {
                    Parameters.Set("to-tag", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets from-tag parameter value. This value is mandatory.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid FromTag value is passed.</exception>
        public string FromTag
        {
            get
            {
                SIP_Parameter parameter = Parameters["from-tag"];
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
                    throw new ArgumentException("FromTag is mandatory and cant be null or empty !");
                }
                else
                {
                    Parameters.Set("from-tag", value);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Join value.</param>
        public SIP_t_Join(string value)
        {
            Parse(value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses "Join" from specified value.
        /// </summary>
        /// <param name="value">SIP "Join" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>value</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "Join" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                Join       = callid *(SEMI join-param)
                join-param = to-tag / from-tag / generic-param
                to-tag     = "to-tag" EQUAL token
                from-tag   = "from-tag" EQUAL token
              
                A Join header MUST contain exactly one to-tag and exactly one from-
                tag, as they are required for unique dialog matching.
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Parse address
            SIP_t_CallID callID = new SIP_t_CallID();
            callID.Parse(reader);
            m_pCallID = callID;

            // Parse parameters
            ParseParameters(reader);

            // Check that to and from tags exist.
            if (Parameters["to-tag"] == null)
            {
                throw new SIP_ParseException("Join value mandatory to-tag value is missing !");
            }
            if (Parameters["from-tag"] == null)
            {
                throw new SIP_ParseException("Join value mandatory from-tag value is missing !");
            }
        }

        /// <summary>
        /// Converts this to valid "Join" value.
        /// </summary>
        /// <returns>Returns "Join" value.</returns>
        public override string ToStringValue()
        {
            /*
                Join       = callid *(SEMI join-param)
                join-param = to-tag / from-tag / generic-param
                to-tag     = "to-tag" EQUAL token
                from-tag   = "from-tag" EQUAL token 
            */

            StringBuilder retVal = new StringBuilder();

            // Add address
            retVal.Append(m_pCallID.ToStringValue());

            // Add parameters
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }

        #endregion
    }
}