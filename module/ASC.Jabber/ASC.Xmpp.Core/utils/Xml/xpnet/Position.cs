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

using System;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.xpnet
{

    #region usings

    #endregion

    /**
 * Represents a position in an entity.
 * A position can be modified by <code>Encoding.movePosition</code>.
 * @see Encoding#movePosition
 * @version $Revision: 1.2 $ $Date: 1998/02/17 04:24:15 $
 */

    /// <summary>
    /// </summary>
    public class Position : ICloneable
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Position()
        {
            LineNumber = 1;
            ColumnNumber = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// </summary>
        public int LineNumber { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Clone()
        {
#if CF
	  throw new util.NotImplementedException();
#else
            throw new NotImplementedException();
#endif
        }

        #endregion

        /**
   * Creates a position for the start of an entity: the line number is
   * 1 and the column number is 0.
   */

        /**
   * Returns the line number.
   * The first line number is 1.
   */

        /**
   * Returns the column number.
   * The first column number is 0.
   * A tab character is not treated specially.
   */

        /**
   * Returns a copy of this position.
   */
    }
}