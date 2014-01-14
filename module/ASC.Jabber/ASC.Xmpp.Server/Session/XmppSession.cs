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
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Utils;

namespace ASC.Xmpp.Server.Session
{
	public class XmppSession
	{
		public string Id
		{
			get;
			private set;
		}

		public Jid Jid
		{
			get;
			private set;
		}

		public bool Active
		{
			get;
			set;
		}

		public XmppStream Stream
		{
			get;
			private set;
		}

		public bool RosterRequested
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			private set;
		}

		public Presence Presence
		{
			get { return presence; }
			internal set
			{
				presence = value;
				Priority = presence != null ? presence.Priority : 0;
			}
		}

		private Presence presence;

		public bool Available
		{
			get { return Presence != null && (Presence.Type == PresenceType.available || Presence.Type == PresenceType.invisible); }
		}

		public ClientInfo ClientInfo
		{
			get;
			private set;
		}

        public DateTime GetRosterTime
        {
            get;
            set;
        }

		public XmppSession(Jid jid, XmppStream stream)
		{
			if (jid == null) throw new ArgumentNullException("jid");
			if (stream == null) throw new ArgumentNullException("stream");

			Id = UniqueId.CreateNewId();
			Jid = jid;
			Stream = stream;
			Active = false;
			RosterRequested = false;
			ClientInfo = new ClientInfo();
		}

        public override string ToString()
        {
            return Jid.ToString();
        } 
	}
}
