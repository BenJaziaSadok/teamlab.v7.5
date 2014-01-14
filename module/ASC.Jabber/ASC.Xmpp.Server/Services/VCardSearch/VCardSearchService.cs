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
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Services.Jabber;

namespace ASC.Xmpp.Server.Services.VCardSearch
{
	class VCardSearchService : XmppServiceBase
	{
		public override void Configure(IDictionary<string, string> properties)
		{
			DiscoInfo.AddIdentity(new DiscoIdentity("service", Name, "jud"));

			Handlers.Add(new VCardSearchHandler());
			Handlers.Add(new VCardHandler());
			Handlers.Add(new ServiceDiscoHandler(Jid));
		}
	}
}