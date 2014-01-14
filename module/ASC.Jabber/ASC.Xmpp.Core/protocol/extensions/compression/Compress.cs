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

namespace ASC.Xmpp.Core.protocol.extensions.compression
{
    // <compress xmlns="http://jabber.org/protocol/compress">
    //      <method>zlib</method>
    // </compress>

    public class Compress : Element
    {
        #region << Constructors >>

        public Compress()
        {
            TagName = "compress";
            Namespace = Uri.COMPRESS;
        }

        /// <summary>
        ///   Constructor with a given method/algorithm for Stream compression
        /// </summary>
        /// <param name="method"> method/algorithm used to compressing the stream </param>
        public Compress(CompressionMethod method) : this()
        {
            Method = method;
        }

        #endregion

        /// <summary>
        ///   method/algorithm used to compressing the stream
        /// </summary>
        public CompressionMethod Method
        {
            set
            {
                if (value != CompressionMethod.Unknown)
                    SetTag("method", value.ToString());
            }
            get { return (CompressionMethod) GetTagEnum("method", typeof (CompressionMethod)); }
        }
    }
}