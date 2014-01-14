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

namespace ASC.Xmpp.Core.utils.Xml.xpnet
{
    /// <summary>
    ///   A token that was parsed.
    /// </summary>
    public class Token
    {
        #region Members

        /// <summary>
        /// </summary>
        private int nameEnd = -1;

        /// <summary>
        /// </summary>
        private char refChar1 = (char) 0;

        /// <summary>
        /// </summary>
        private char refChar2 = (char) 0;

        /// <summary>
        /// </summary>
        private int tokenEnd = -1;

        #endregion

        #region Properties

        /// <summary>
        ///   The end of the current token's name, in relation to the beginning of the buffer.
        /// </summary>
        public int NameEnd
        {
            get { return nameEnd; }

            set { nameEnd = value; }
        }

        // public char RefChar
        // {
        // get {return refChar1;}
        // }

        /// <summary>
        ///   The parsed-out character. &amp; for &amp;amp;
        /// </summary>
        public char RefChar1
        {
            get { return refChar1; }

            set { refChar1 = value; }
        }

        /// <summary>
        ///   The second of two parsed-out characters. TODO: find example.
        /// </summary>
        public char RefChar2
        {
            get { return refChar2; }

            set { refChar2 = value; }
        }

        /// <summary>
        ///   The end of the current token, in relation to the beginning of the buffer.
        /// </summary>
        public int TokenEnd
        {
            get { return tokenEnd; }

            set { tokenEnd = value; }
        }

        #endregion

        /*
        public void getRefCharPair(char[] ch, int off) {
            ch[off] = refChar1;
            ch[off + 1] = refChar2;
        }
        */
    }
}