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
using System.Collections.Generic;
using System.Text;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IXmppConnection
	{
		string Id
		{
			get;
		}

		void Reset();

		void Close();

		void Send(Node node, Encoding encoding);

		void Send(string text, Encoding encoding);

		void BeginReceive();

		void SetStreamTransformer(IStreamTransformer transformer);

		event EventHandler<XmppStreamStartEventArgs> XmppStreamStart;

		event EventHandler<XmppStreamEventArgs> XmppStreamElement;

		event EventHandler<XmppStreamEndEventArgs> XmppStreamEnd;

		event EventHandler<XmppConnectionCloseEventArgs> Closed;
	}

	public class XmppStreamEventArgs : EventArgs
	{
		public string ConnectionId
		{
			get;
			private set;
		}

		public Node Node
		{
			get;
			private set;
		}

		public XmppStreamEventArgs(string connectionId, Node node)
		{
			if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");
			if (node == null) throw new ArgumentNullException("node");

			ConnectionId = connectionId;
			Node = node;
		}
	}

	public class XmppStreamStartEventArgs : XmppStreamEventArgs
	{
		public string Namespace
		{
			get;
			private set;
		}

		public XmppStreamStartEventArgs(string connectionId, Node node, string ns)
			: base(connectionId, node)
		{
			Namespace = ns;
		}
	}

	public class XmppStreamEndEventArgs : EventArgs
	{
		public string ConnectionId
		{
			get;
			private set;
		}

		public ICollection<Node> NotSendedBuffer
		{
			get;
			private set;
		}

		public XmppStreamEndEventArgs(string connectionId, IEnumerable<Node> notSendedBuffer)
		{
            
			if (string.IsNullOrEmpty(connectionId)) throw new ArgumentNullException("connectionId");
		    
			ConnectionId = connectionId;
			if (notSendedBuffer == null)
			{
				NotSendedBuffer = new List<Node>();
			}
			else
			{
				NotSendedBuffer = new List<Node>(notSendedBuffer);
			}
		}
	}

	public class XmppConnectionCloseEventArgs : EventArgs
	{

	}
}
