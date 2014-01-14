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
using System.Runtime.Serialization;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.utils.Xml.Dom;
using StanzaError = ASC.Xmpp.Core.protocol.client.Error;
using StreamError = ASC.Xmpp.Core.protocol.Error;

namespace ASC.Xmpp.Server
{
    public class JabberException : Exception
    {
        public StreamErrorCondition StreamErrorCondition
        {
            get;
            private set;
        }


        public ErrorCode ErrorCode
        {
            get;
            private set;
        }

        public bool CloseStream
        {
            get;
            private set;
        }

        public bool StreamError
        {
            get;
            private set;
        }

        public JabberException(string message, Exception innerException)
            : base(message, innerException)
        {
            StreamError = false;
            ErrorCode = ErrorCode.InternalServerError;
        }

        public JabberException(StreamErrorCondition streamErrorCondition)
            : this(streamErrorCondition, true)
        {

        }

        public JabberException(StreamErrorCondition streamErrorCondition, bool closeStream)
            : base()
        {
            StreamError = true;
            CloseStream = closeStream;
            this.StreamErrorCondition = streamErrorCondition;
        }

        public JabberException(ErrorCode errorCode)
            : base()
        {
            StreamError = false;
            CloseStream = false;
            this.ErrorCode = errorCode;
        }

        protected JabberException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public virtual Element ToElement()
        {
            return StreamError ? (Element)new StreamError(StreamErrorCondition) : (Element)new StanzaError(ErrorCode);
        }
    }
}
