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
    /// This class represent ABNF "concatenation". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Concatenation
    {
        #region Members

        private readonly List<ABNF_Repetition> m_pItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets concatenation items.
        /// </summary>
        public List<ABNF_Repetition> Items
        {
            get { return m_pItems; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ABNF_Concatenation()
        {
            m_pItems = new List<ABNF_Repetition>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Concatenation Parse(StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // concatenation  =  repetition *(1*c-wsp repetition)
            // repetition     =  [repeat] element

            ABNF_Concatenation retVal = new ABNF_Concatenation();

            while (true)
            {
                ABNF_Repetition item = ABNF_Repetition.Parse(reader);
                if (item != null)
                {
                    retVal.m_pItems.Add(item);
                }
                    // We reached end of string.
                else if (reader.Peek() == -1)
                {
                    break;
                }
                    // We have next concatenation item.
                else if (reader.Peek() == ' ')
                {
                    reader.Read();
                }
                    // We have unexpected value, probably concatenation ends.
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