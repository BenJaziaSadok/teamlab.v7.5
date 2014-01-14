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

using ASC.Xmpp.Core.protocol.iq.bind;
using ASC.Xmpp.Core.protocol.sasl;
using ASC.Xmpp.Core.protocol.stream.feature;
using ASC.Xmpp.Core.protocol.stream.feature.compression;
using ASC.Xmpp.Core.protocol.tls;
using ASC.Xmpp.Core.utils.Xml.Dom;

//</stream:features>
// <stream:features>
//		<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//			<mechanism>DIGEST-MD5</mechanism>
//			<mechanism>PLAIN</mechanism>
//		</mechanisms>
// </stream:features>

// <stream:features>
//		<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'>
//			<required/>
//		</starttls>
//		<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//			<mechanism>DIGEST-MD5</mechanism>
//			<mechanism>PLAIN</mechanism>
//		</mechanisms>
// </stream:features>

namespace ASC.Xmpp.Core.protocol.stream
{
    /// <summary>
    ///   Summary description for Features.
    /// </summary>
    public class Features : Element
    {
        public Features()
        {
            TagName = "features";
            Namespace = Uri.STREAM;
        }

        public StartTls StartTls
        {
            get { return SelectSingleElement(typeof (StartTls)) as StartTls; }
            set
            {
                if (HasTag(typeof (StartTls)))
                    RemoveTag(typeof (StartTls));

                if (value != null)
                    AddChild(value);
            }
        }

        public Bind Bind
        {
            get { return SelectSingleElement(typeof (Bind)) as Bind; }
            set
            {
                if (HasTag(typeof (Bind)))
                    RemoveTag(typeof (Bind));

                if (value != null)
                    AddChild(value);
            }
        }

        // <stream:stream from="beta.soapbox.net" xml:lang="de" id="373af7e9-6107-4729-8cea-e8b8ea05ceea" xmlns="jabber:client" version="1.0" xmlns:stream="http://etherx.jabber.org/streams">

        // <stream:features xmlns:stream="http://etherx.jabber.org/streams">
        //      <compression xmlns="http://jabber.org/features/compress"><method>zlib</method></compression>
        //      <starttls xmlns="urn:ietf:params:xml:ns:xmpp-tls" />
        //      <register xmlns="http://jabber.org/features/iq-register" />
        //      <auth xmlns="http://jabber.org/features/iq-auth" />
        //      <mechanisms xmlns="urn:ietf:params:xml:ns:xmpp-sasl">
        //          <mechanism>PLAIN</mechanism>
        //          <mechanism>DIGEST-MD5</mechanism>
        //          <mechanism>ANONYMOUS</mechanism>
        //      </mechanisms>
        // </stream:features>


        public Compression Compression
        {
            get { return SelectSingleElement(typeof (Compression)) as Compression; }
            set
            {
                if (HasTag(typeof (Compression)))
                    RemoveTag(typeof (Compression));

                if (value != null)
                    AddChild(value);
            }
        }

        public Register Register
        {
            get { return SelectSingleElement(typeof (Register)) as Register; }
            set
            {
                if (HasTag(typeof (Register)))
                    RemoveTag(typeof (Register));

                if (value != null)
                    AddChild(value);
            }
        }

        public Mechanisms Mechanisms
        {
            get { return SelectSingleElement(typeof (Mechanisms)) as Mechanisms; }
            set
            {
                if (HasTag(typeof (Mechanisms)))
                    RemoveTag(typeof (Mechanisms));

                if (value != null)
                    AddChild(value);
            }
        }

        public bool SupportsBind
        {
            get { return Bind != null ? true : false; }
        }

        public bool SupportsStartTls
        {
            get { return StartTls != null ? true : false; }
        }

        /// <summary>
        ///   Is Stream Compression supported?
        /// </summary>
        public bool SupportsCompression
        {
            get { return Compression != null ? true : false; }
        }

        /// <summary>
        ///   Is Registration supported?
        /// </summary>
        public bool SupportsRegistration
        {
            get { return Register != null ? true : false; }
        }
    }
}