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
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Users
{
	public class UserManager
	{
		private StorageManager storageManager;

		private IUserStore userStore;

		private IUserStore UserStore
		{
			get
			{
				if (userStore == null)
				{
					lock (this)
					{
						if (userStore == null) userStore = storageManager.UserStorage;
					}
				}
				return userStore;
			}
		}

		public UserManager(StorageManager storageManager)
		{
			if (storageManager == null) throw new ArgumentNullException("storageManager");
			this.storageManager = storageManager;
		}

		public bool IsUserExists(Jid jid)
		{
			return GetUser(jid) != null;
		}

		public User GetUser(Jid jid)
		{
			return UserStore.GetUser(jid);
		}

		public ICollection<User> GetUsers(string domain)
		{
			return UserStore.GetUsers(domain);
		}

		public void SaveUser(User user)
		{
			UserStore.SaveUser(user);
		}

		public void RemoveUser(Jid jid)
		{
			UserStore.RemoveUser(jid);
		}
	}
}