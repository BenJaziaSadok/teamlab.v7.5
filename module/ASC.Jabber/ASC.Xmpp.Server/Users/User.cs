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

namespace ASC.Xmpp.Server.Users
{
	public class User
	{
		public Jid Jid
		{
			get;
			private set;
		}

		public string Password
		{
			get;
			set;
		}

		public bool IsAdmin
		{
			get;
			set;
		}

		public User(Jid jid)
			: this(jid, null)
		{

		}

		public User(Jid jid, string password)
			: this(jid, password, true)
		{

		}

		public User(Jid jid, string password, bool admin)
		{
			if (jid == null) throw new ArgumentNullException("jid");

			Jid = jid;
			Password = password;
			IsAdmin = admin;
		}

		public override string ToString()
		{
			return Jid.ToString();
		}

		public override int GetHashCode()
		{
			return Jid.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var u = obj as User;
			return u != null && Jid.Equals(u.Jid);
		}
	}
}