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

    // Step 4: Client sends the STARTTLS command to server:
    // <starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>

    /// <summary>
    ///   Summary description for starttls.
    /// </summary>
    public class StartTls : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public StartTls()
        {
            TagName = "starttls";
            Namespace = Uri.TLS;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public bool Required
        {
            get { return HasTag("required"); }

            set
            {
                if (value == false)
                {
                    if (HasTag("required"))
                    {
                        RemoveTag("required");
                    }
                }
                else
                {
                    if (!HasTag("required"))
                    {
                        SetTag("required");
                    }
                }
            }
        }

        #endregion
    }
}