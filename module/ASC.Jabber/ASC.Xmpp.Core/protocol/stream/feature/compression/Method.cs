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
using ASC.Xmpp.Core.protocol.extensions.compression;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.stream.feature.compression
{
    public class Method : Element
    {
        #region << Constructors >>

        public Method()
        {
            TagName = "method";
            Namespace = Uri.FEATURE_COMPRESS;
        }

        public Method(CompressionMethod method) : this()
        {
            Value = method.ToString();
        }

        #endregion

        /*
         *  <compression xmlns='http://jabber.org/features/compress'>
         *      <method>zlib</method>
         *  </compression>
         * 
         * <stream:features>
         *      <starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>
         *      <compression xmlns='http://jabber.org/features/compress'>
         *          <method>zlib</method>
         *          <method>lzw</method>
         *      </compression>
         * </stream:features>
         */

        public CompressionMethod CompressionMethod
        {
            get
            {
#if CF
				return (CompressionMethod) util.Enum.Parse(typeof(CompressionMethod), this.Value, true);
#else
                return (CompressionMethod) Enum.Parse(typeof (CompressionMethod), Value, true);
#endif
            }
            set { Value = value.ToString(); }
        }
    }
}