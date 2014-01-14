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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Core.utils.Xml.Dom;
using RosterItem = ASC.Xmpp.Core.protocol.iq.roster.RosterItem;

namespace ASC.Xmpp.Server.Storage
{
	public class UserRosterItem
	{
		public Jid Jid
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			set;
		}

		public SubscriptionType Subscribtion
		{
			get;
			set;
		}

		public AskType Ask
		{
			get;
			set;
		}

		public List<string> Groups
		{
			get;
			private set;
		}

		public UserRosterItem(Jid jid)
		{
			if (jid == null) throw new ArgumentNullException("jid");

			Jid = new Jid(jid.Bare.ToLowerInvariant());
			Groups = new List<string>();
		}

		public RosterItem ToRosterItem()
		{
			var ri = new RosterItem(Jid, Name)
			{
				Subscription = Subscribtion,
				Ask = Ask,
			};
			Groups.ForEach(g => ri.AddGroup(g));
			return ri;
		}

		public static UserRosterItem FromRosterItem(RosterItem ri)
		{
			var item = new UserRosterItem(ri.Jid)
			{
				Name = ri.Name,
				Ask = ri.Ask,
				Subscribtion = ri.Subscription,
			};
			foreach (Element element in ri.GetGroups())
			{
				item.Groups.Add(element.Value);
			}
			return item;
		}

		public IQ GetRosterIq(Jid to)
		{
			var iq = new IQ(IqType.set);
			var roster = new Roster();
			roster.AddRosterItem(ToRosterItem());
			iq.Query = roster;
			iq.To = to.BareJid;
			return iq;
		}

		public override string ToString()
		{
			return string.IsNullOrEmpty(Name) ? Jid.ToString() : Name;
		}

		public override bool Equals(object obj)
		{
			var i = obj as UserRosterItem;
			return i != null && i.Jid == Jid;
		}

		public override int GetHashCode()
		{
			return Jid.GetHashCode();
		}
	}
}