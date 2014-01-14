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

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Copyright (c) 2003-2008 by AG-Software 											 *
 * All Rights Reserved.																 *
 * Contact information for AG-Software is available at http://www.ag-software.de	 *
 *																					 *
 * Licence:																			 *
 * The agsXMPP SDK is released under a dual licence									 *
 * agsXMPP can be used under either of two licences									 *
 * 																					 *
 * A commercial licence which is probably the most appropriate for commercial 		 *
 * corporate use and closed source projects. 										 *
 *																					 *
 * The GNU Public License (GPL) is probably most appropriate for inclusion in		 *
 * other open source projects.														 *
 *																					 *
 * See README.html for details.														 *
 *																					 *
 * For general enquiries visit our website at:										 *
 * http://www.ag-software.de														 *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */ 

using System;
using System.Collections;

using ASC.Xmpp.protocol;
using ASC.Xmpp.protocol.iq;
using ASC.Xmpp.protocol.client;

namespace ASC.Xmpp.protocol.iq.roster
{
    using client;

    /// <summary>
	/// Helper Class that makes it easier to manage your roster
	/// </summary>
	public class RosterManager
	{
		private XmppClientConnection	m_connection	= null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="con">The XmppClientConnection on which the RosterManager should send the packets</param>
		public RosterManager(XmppClientConnection con)
		{		
			m_connection = con;
		}

		/// <summary>
		/// Removes a Rosteritem from the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		public void RemoveRosterItem(Jid jid)
		{
			RosterIq riq = new RosterIq();
			riq.Type = IqType.set;
				
			RosterItem ri = new RosterItem();
			ri.Jid = jid;
			ri.Subscription = SubscriptionType.remove;

			riq.Query.AddRosterItem(ri);
				
			m_connection.Send(riq);
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		public void AddRosterItem(Jid jid)
		{
			AddRosterItem(jid, null, new string[] {});
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		public void UpdateRosterItem(Jid jid)
		{
			AddRosterItem(jid, null, new string[] {});
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		/// <param name="nickname">Nickname for the RosterItem</param>
		public void AddRosterItem(Jid jid, string nickname)
		{
			AddRosterItem(jid, nickname, new string[] {});
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="nickname"></param>
		public void UpdateRosterItem(Jid jid, string nickname)
		{
			AddRosterItem(jid, nickname, new string[] {});
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		/// <param name="nickname">Nickname for the RosterItem</param>
		/// <param name="group">The group to which the roteritem should be added</param>
		public void AddRosterItem(Jid jid, string nickname, string group)
		{
			AddRosterItem(jid, nickname, new string[] {group});
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="nickname"></param>
		/// <param name="group"></param>
		public void UpdateRosterItem(Jid jid, string nickname, string group)
		{
			AddRosterItem(jid, nickname, new string[] {group});
		}

		/// <summary>
		/// Add a Rosteritem to the Roster
		/// </summary>
		/// <param name="jid">The BARE jid of the rosteritem that should be removed</param>
		/// <param name="nickname">Nickname for the RosterItem</param>
		/// <param name="group">An Array of groups when you want to add the Rosteritem to multiple groups</param>
		public void AddRosterItem(Jid jid, string nickname, string[] group)
		{
			RosterIq riq = new RosterIq();
			riq.Type = IqType.set;
				
			RosterItem ri = new RosterItem();
			ri.Jid	= jid;
			
			if (nickname != null)
				ri.Name	= nickname;
			
			foreach (string g in group)
			{
				ri.AddGroup(g);			
			}

			riq.Query.AddRosterItem(ri);
				
			m_connection.Send(riq);
		}

		/// <summary>
		/// Update a Rosteritem
		/// </summary>
		/// <param name="jid"></param>
		/// <param name="nickname"></param>
		/// <param name="group"></param>
		public void UpdateRosterItem(Jid jid, string nickname, string[] group)
		{
			AddRosterItem(jid, nickname, group);
		}
		
	}
}
