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

using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.component;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
    public class DbOfflineStoreTest : DbBaseTest
	{
		private DbOfflineStore store;
		private Jid jid = new Jid("james.bond@london.uk/PUB");

        public DbOfflineStoreTest()
		{
			store = new DbOfflineStore();
            store.Configure(GetConfiguration());
		}

		[Test]
		public void OfflineMessagesTest()
		{
			store.RemoveAllOfflineMessages(jid);
			var messages = store.GetOfflineMessages(jid);
			CollectionAssert.IsEmpty(messages);

			var m = new Message()
			{
				To = jid,
				From = new Jid("y"),
				Body = "xy"
			};
			store.SaveOfflineMessages(m);

			messages = store.GetOfflineMessages(jid);
			Assert.AreEqual(1, messages.Count);
			Assert.AreEqual(m.To, messages[0].To);
			Assert.AreEqual(m.From, messages[0].From);
			Assert.AreEqual(m.Body, messages[0].Body);

			store.SaveOfflineMessages(m);
			messages = store.GetOfflineMessages(jid);
			Assert.AreEqual(2, messages.Count);

			messages = store.GetOfflineMessages(new Jid("y"));
			CollectionAssert.IsEmpty(messages);

			store.RemoveAllOfflineMessages(jid);

			store.SaveOfflineMessages(new Message());
			messages = store.GetOfflineMessages(jid);
			CollectionAssert.IsEmpty(messages);
		}

		[Test]
		public void OfflinePresencesTest()
		{
			store.RemoveAllOfflinePresences(jid);
			var presences = store.GetOfflinePresences(jid);
			CollectionAssert.IsEmpty(presences);

			var p = new Presence()
			{
				To = jid,
				From = new Jid("y"),
			};
			store.SaveOfflinePresence(p);

			presences = store.GetOfflinePresences(jid);
			Assert.AreEqual(1, presences.Count);
			Assert.AreEqual(p.To.Bare, presences[0].To.ToString());
			Assert.AreEqual(p.From.Bare, presences[0].From.ToString());
			Assert.AreEqual(p.Type, presences[0].Type);

			store.RemoveAllOfflinePresences(jid);
			presences = store.GetOfflinePresences(jid);
			CollectionAssert.IsEmpty(presences);
		}

		[Test]
		public void LastActivityTest()
		{
			store.SaveLastActivity(jid, new LastActivity("OK"));
		
			var la = store.GetLastActivity(jid);
			Assert.AreEqual("OK", la.Status);

			la = store.GetLastActivity(new Jid("x"));
			Assert.IsNull(la);
		}
	}
}
