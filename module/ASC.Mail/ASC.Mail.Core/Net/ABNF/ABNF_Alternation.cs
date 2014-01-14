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
    using System.Collections.Generic;
    using System.IO;

    #endregion

    /// <summary>
    /// This class represent ABNF "alternation". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Alternation
    {
        #region Members

        private readonly List<ABNF_Concatenation> m_pItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets alternation items.
        /// </summary>
        public List<ABNF_Concatenation> Items
        {
            get { return m_pItems; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ABNF_Alternation()
        {
            m_pItems = new List<ABNF_Concatenation>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Alternation Parse(StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // alternation = concatenation *(*c-wsp "/" *c-wsp concatenation)

            ABNF_Alternation retVal = new ABNF_Alternation();

            while (true)
            {
                ABNF_Concatenation item = ABNF_Concatenation.Parse(reader);
                if (item != null)
                {
                    retVal.m_pItems.Add(item);
                }

                // We reached end of string.
                if (reader.Peek() == -1)
                {
                    break;
                }
                    // We have next alternation item.
                else if (reader.Peek() == '/')
                {
                    reader.Read();
                }
                    // We have unexpected value, probably alternation ends.
                else
                {
                    break;
                }
            }

            return retVal;
        }

        #endregion
    }
}