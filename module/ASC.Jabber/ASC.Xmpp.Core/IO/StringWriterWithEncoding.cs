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

#region using

using System.IO;
using System.Text;

#endregion

namespace ASC.Xmpp.Core.IO
{

    #region usings

    #endregion

    /// <summary>
    ///   This class is inherited from the StringWriter Class The standard StringWriter class supports no encoding With this Class we can set the Encoding of a StringWriter in the Constructor
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly Encoding m_Encoding;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="encoding"> </param>
        public StringWriterWithEncoding(Encoding encoding)
        {
            m_Encoding = encoding;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public override Encoding Encoding
        {
            get { return m_Encoding; }
        }

        #endregion
    }
}