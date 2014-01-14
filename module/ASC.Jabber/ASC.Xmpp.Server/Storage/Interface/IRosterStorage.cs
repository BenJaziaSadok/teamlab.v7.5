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
using agsXMPP.protocol.iq.roster;

namespace ASC.Xmpp.Server.storage.Interface
{
	public interface IRosterStore
	{

		List<UserRosterItem> GetRosterItems(string userName);

		List<UserRosterItem> GetRosterItems(string userName, SubscriptionType subscriptionType);

		UserRosterItem GetRosterItem(string userName, string jid);

		void SaveOrUpdateRosterItem(string userName, UserRosterItem item);

		void RemoveRosterItem(string userName, string jid);
	}
}
