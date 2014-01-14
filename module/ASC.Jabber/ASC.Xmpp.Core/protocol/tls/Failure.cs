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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.tls
{

    #region usings

    #endregion

    // Step 5 (alt): Server informs client that TLS negotiation has failed and closes both stream and TCP connection:

    // <failure xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>
    // </stream:stream>

    /// <summary>
    ///   Summary description for Failure.
    /// </summary>
    public class Failure : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Failure()
        {
            TagName = "failure";
            Namespace = Uri.TLS;
        }

        #endregion
    }
}