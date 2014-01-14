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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Server.Services.Jabber;

namespace ASC.Xmpp.Server.Services.Multicast
{
	class MulticastService : XmppServiceBase
	{
		public override void Configure(IDictionary<string, string> properties)
		{
			DiscoInfo.AddIdentity(new DiscoIdentity("text", Name, "Multicast Service"));

            DiscoInfo.AddFeature(new DiscoFeature(Uri.ADDRESS));

			Handlers.Add(new MulticastHandler());
			Handlers.Add(new VCardHandler());
			Handlers.Add(new ServiceDiscoHandler(Jid));
		}
	}
}