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

namespace ASC.Mail.Net.ABNF
{
    #region usings

    using System;
    using System.IO;

    #endregion

    /// <summary>
    /// This class represent ABNF "group". Defined in RFC 5234 4.
    /// </summary>
    public class ABFN_Group : ABNF_Element
    {
        #region Members

        private ABNF_Alternation m_pAlternation;

        #endregion

        #region Properties

        /// <summary>
        /// Gets option alternation elements.
        /// </summary>
        public ABNF_Alternation Alternation
        {
            get { return m_pAlternation; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABFN_Group Parse(StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // group = "(" *c-wsp alternation *c-wsp ")"

            if (reader.Peek() != '(')
            {
                throw new ParseException("Invalid ABNF 'group' value '" + reader.ReadToEnd() + "'.");
            }

            // Eat "(".
            reader.Read();

            // TODO: *c-wsp

            ABFN_Group retVal = new ABFN_Group();

            // We reached end of stream, no closing ")".
            if (reader.Peek() == -1)
            {
                throw new ParseException("Invalid ABNF 'group' value '" + reader.ReadToEnd() + "'.");
            }

            retVal.m_pAlternation = ABNF_Alternation.Parse(reader);

            // We don't have closing ")".
            if (reader.Peek() != ')')
            {
                throw new ParseException("Invalid ABNF 'group' value '" + reader.ReadToEnd() + "'.");
            }
            else
            {
                reader.Read();
            }

            return retVal;
        }

        #endregion
    }
}