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

namespace ASC.Xmpp.Core.protocol
{
    /// <summary>
    ///   stream:stream Element This is the first Element we receive from the server. It encloses our whole xmpp session.
    /// </summary>
    public class Stream : Base.Stream
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Stream()
        {
            Namespace = Uri.STREAM;
        }

        #endregion
    }
}