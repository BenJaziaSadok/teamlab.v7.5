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

using ASC.Xmpp.Core.protocol.extensions.bookmarks;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.iq.@private
{
    /// <summary>
    ///   Private XML Storage JEP-0049
    /// </summary>
    /// <remarks>
    ///   A Jabber client can store any arbitrary XML on the server side by sending an iq stanza of type "set" to the server with a query child scoped by the 'jabber:iq:private' namespace. The query element MAY contain any arbitrary XML fragment as long as the root element of that fragment is scoped by its own namespace. The data can then be retrieved by sending an iq stanza of type "get" with a query child scoped by the 'jabber:iq:private' namespace, which in turn contains a child element scoped by the namespace used for storage of that fragment. Using this method, Jabber entities can store private data on the server and retrieve it whenever necessary. The data stored might be anything, as long as it is valid XML. One typical usage for this namespace is the server-side storage of client-specific preferences; another is Bookmark Storage.
    /// </remarks>
    public class Private : Element
    {
        public Private()
        {
            TagName = "query";
            Namespace = Uri.IQ_PRIVATE;
        }

        /// <summary>
        ///   The <see cref="extensions.bookmarks.Storage">Storage</see> object
        /// </summary>
        public Storage Storage
        {
            get { return SelectSingleElement(typeof (Storage)) as Storage; }
            set
            {
                if (HasTag(typeof (Storage)))
                    RemoveTag(typeof (Storage));

                if (value != null)
                    AddChild(value);
            }
        }
    }
}