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

using System.Collections.Generic;
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Services;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.protocol.iq.disco;

namespace ASC.Xmpp.Server.Jabber
{
	class TestService : XmppServiceBase
	{
		public TestService()
		{
			Name = "Multi User Chat";
			DiscoInfo.AddIdentity(new DiscoIdentity("muc", Name, "im"));
		}
	}
}
