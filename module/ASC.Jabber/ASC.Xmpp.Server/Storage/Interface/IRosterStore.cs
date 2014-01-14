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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.roster;

namespace ASC.Xmpp.Server.Storage.Interface
{
	public interface IRosterStore
	{

		List<UserRosterItem> GetRosterItems(Jid rosterJid);

		List<UserRosterItem> GetRosterItems(Jid rosterJid, SubscriptionType subscriptionType);

		UserRosterItem GetRosterItem(Jid rosterJid, Jid itemJid);

		UserRosterItem SaveRosterItem(Jid rosterJid, UserRosterItem item);

		void RemoveRosterItem(Jid rosterJid, Jid itemJid);
	}
}
