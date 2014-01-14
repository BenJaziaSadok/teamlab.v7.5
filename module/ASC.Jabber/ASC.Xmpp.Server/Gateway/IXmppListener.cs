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
using ASC.Xmpp.Common.Configuration;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Server.Gateway
{
	public interface IXmppListener : IConfigurable
	{
		string Name
		{
			get;
			set;
		}

		void Start();

		void Stop();

		IXmppConnection GetXmppConnection(string connectionId);

		event EventHandler<XmppConnectionOpenEventArgs> OpenXmppConnection;
	}

	public class XmppConnectionOpenEventArgs : EventArgs
	{
		public IXmppConnection XmppConnection
		{
			get;
			private set;
		}

		public XmppConnectionOpenEventArgs(IXmppConnection xmppConnection)
		{
			if (xmppConnection == null) throw new ArgumentNullException("xmppConnection");

			XmppConnection = xmppConnection;
		}
	}
}
