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
    /// Implements SIP "Replaces" value. Defined in RFC 3891.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3891 Syntax:
    ///     Replaces        = callid *(SEMI replaces-param)
    ///     replaces-param  = to-tag / from-tag / early-flag / generic-param
    ///     to-tag          = "to-tag" EQUAL token
    ///     from-tag        = "from-tag" EQUAL token
    ///     early-flag      = "early-only"
    /// </code>
    /// </remarks>
    public class SIP_t_Replaces : SIP_t_ValueWithParams
    {
        #region Members

        private string m_CallID = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets call id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public string CallID
        {
            get { return m_CallID; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("CallID");
                }

                m_CallID = value;
            }
        }

        /// <summary>
        /// Gets or sets Replaces 'to-tag' parameter. Value null means not specified.
        /// </summary>
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
                if (value == null)
                {
                    Parameters.Remove("to-tag");
                }
                else
                {
                    Parameters.Set("to-tag", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets Replaces 'from-tag' parameter. Value null means not specified.
        /// </summary>
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
                if (value == null)
                {
                    Parameters.Remove("from-tag");
                }
                else
                {
                    Parameters.Set("from-tag", value);
                }
            }
        }

        /// <summary>
        /// Gets or sets Replaces 'early-flag' parameter.
        /// </summary>
        public bool EarlyFlag
        {
            get
            {
                if (Parameters.Contains("early-only"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                if (!value)
                {
                    Parameters.Remove("early-only");
                }
                else
                {
                    Parameters.Set("early-only", null);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses "Replaces" from specified value.
        /// </summary>
        /// <param name="value">SIP "Replaces" value.</param>
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
        /// Parses "Replaces" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                Replaces        = callid *(SEMI replaces-param)
                replaces-param  = to-tag / from-tag / early-flag / generic-param
                to-tag          = "to-tag" EQUAL token
                from-tag        = "from-tag" EQUAL token
                early-flag      = "early-only"    
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // callid
            string word = reader.ReadWord();
            if (word == null)
            {
                throw new SIP_ParseException("Replaces 'callid' value is missing !");
            }
            m_CallID = word;

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "Replaces" value.
        /// </summary>
        /// <returns>Returns "Replaces" value.</returns>
        public override string ToStringValue()
        {
            /*
                Replaces        = callid *(SEMI replaces-param)
                replaces-param  = to-tag / from-tag / early-flag / generic-param
                to-tag          = "to-tag" EQUAL token
                from-tag        = "from-tag" EQUAL token
                early-flag      = "early-only"    
            */

            StringBuilder retVal = new StringBuilder();

            // delta-seconds
            retVal.Append(m_CallID);

            // Add parameters
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }

        #endregion
    }
}