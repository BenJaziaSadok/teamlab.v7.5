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

    using System.Text;

    #endregion

    /// <summary>
    /// This base class for SIP data types what has parameters support.
    /// </summary>
    public abstract class SIP_t_ValueWithParams : SIP_t_Value
    {
        #region Members

        private readonly SIP_ParameterCollection m_pParameters;

        #endregion

        #region Properties

        /// <summary>
        /// Gets via parameters.
        /// </summary>
        public SIP_ParameterCollection Parameters
        {
            get { return m_pParameters; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SIP_t_ValueWithParams()
        {
            m_pParameters = new SIP_ParameterCollection();
        }

        #endregion

        /// <summary>
        /// Parses parameters from specified reader. Reader position must be where parameters begin.
        /// </summary>
        /// <param name="reader">Reader from where to read parameters.</param>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        protected void ParseParameters(StringReader reader)
        {
            // Remove all old parameters.
            m_pParameters.Clear();

            // Parse parameters
            while (reader.Available > 0)
            {
                reader.ReadToFirstChar();

                // We have parameter
                if (reader.SourceString.StartsWith(";"))
                {
                    reader.ReadSpecifiedLength(1);
                    string paramString = reader.QuotedReadToDelimiter(new[] {';', ','}, false);
                    if (paramString != "")
                    {
                        string[] name_value = paramString.Split(new[] {'='}, 2);
                        if (name_value.Length == 2)
                        {
                            Parameters.Add(name_value[0], name_value[1]);
                        }
                        else
                        {
                            Parameters.Add(name_value[0], null);
                        }
                    }
                }
                    // Next value
                else if (reader.SourceString.StartsWith(","))
                {
                    break;
                }
                    // Unknown data
                else
                {
                    throw new SIP_ParseException("Unexpected value '" + reader.SourceString + "' !");
                }
            }
        }

        /// <summary>
        /// Convert parameters to valid parameters string.
        /// </summary>
        /// <returns>Returns parameters string.</returns>
        protected string ParametersToString()
        {
            StringBuilder retVal = new StringBuilder();
            foreach (SIP_Parameter parameter in m_pParameters)
            {
                if (!string.IsNullOrEmpty(parameter.Value))
                {
                    retVal.Append(";" + parameter.Name + "=" + parameter.Value);
                }
                else
                {
                    retVal.Append(";" + parameter.Name);
                }
            }

            return retVal.ToString();
        }
    }
}