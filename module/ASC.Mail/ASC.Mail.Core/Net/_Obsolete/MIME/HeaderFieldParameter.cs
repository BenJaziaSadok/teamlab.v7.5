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

namespace ASC.Mail.Net.Mime
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// Header field parameter.
    /// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
    public class HeaderFieldParameter
    {
        #region Members

        private readonly string m_Name = "";
        private readonly string m_Value = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets header field parameter name.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets header field parameter name.
        /// </summary>
        public string Value
        {
            get { return m_Value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="parameterName">Header field parameter name.</param>
        /// <param name="parameterValue">Header field parameter value.</param>
        public HeaderFieldParameter(string parameterName, string parameterValue)
        {
            m_Name = parameterName;
            m_Value = parameterValue;
        }

        #endregion
    }
}