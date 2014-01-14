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

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.component
{
    /// <summary>
    ///   Summary description for Error.
    /// </summary>
    public class Error : client.Error
    {
        public Error()
        {
            Namespace = Uri.ACCEPT;
        }

        public Error(int code)
            : base(code)
        {
            Namespace = Uri.ACCEPT;
        }

        public Error(ErrorCode code)
            : base(code)
        {
            Namespace = Uri.ACCEPT;
        }

        public Error(ErrorType type)
            : base(type)
        {
            Namespace = Uri.ACCEPT;
        }

        /// <summary>
        ///   Creates an error Element according the the condition The type attrib as added automatically as decribed in the XMPP specs This is the prefered way to create error Elements
        /// </summary>
        /// <param name="condition"> </param>
        public Error(ErrorCondition condition)
            : base(condition)
        {
            Namespace = Uri.ACCEPT;
        }
    }
}