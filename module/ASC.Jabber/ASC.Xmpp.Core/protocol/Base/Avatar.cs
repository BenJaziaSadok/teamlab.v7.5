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

using System;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.Base
{

    #region usings

    #endregion

    // Avatar is in multiple Namespaces. So better to work with a Base class

    /// <summary>
    ///   Summary description for Avatar.
    /// </summary>
    public class Avatar : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Avatar()
        {
            TagName = "query";
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public byte[] Data
        {
            get
            {
                if (HasTag("data"))
                {
                    return Convert.FromBase64String(GetTag("data"));
                }
                else
                {
                    return null;
                }
            }

            set { SetTag("data", Convert.ToBase64String(value, 0, value.Length)); }
        }

        /// <summary>
        /// </summary>
        public string MimeType
        {
            get
            {
                Element data = SelectSingleElement("data");
                if (data != null)
                {
                    return GetAttribute("mimetype");
                }
                else
                {
                    return null;
                }
            }

            set
            {
                Element data = SelectSingleElement("data");
                if (data != null)
                {
                    SetAttribute("mimetype", value);
                }
            }
        }

        #endregion
    }
}