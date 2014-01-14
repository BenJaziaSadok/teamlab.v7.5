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
using ASC.Xmpp.Core.protocol.iq.roster;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
    public class DbRosterStoreTest : DbBaseTest
	{
		private DbRosterStore store;
		private Jid jid = new Jid("james.bond@london.uk/PUB");

        public DbRosterStoreTest()
		{
			store = new DbRosterStore();
            store.Configure(GetConfiguration());
        }

		[Test]
		public void RosterTest()
		{
			foreach (var i in store.GetRosterItems(jid)) store.RemoveRosterItem(jid, i.Jid);

			var roster = store.GetRosterItems(jid);
			CollectionAssert.IsEmpty(roster);

			var r = new UserRosterItem(new Jid("x"))
			{
				Name = "y",
				Subscribtion = SubscriptionType.from,
				Ask = AskType.subscribe,
			};
			r.Groups.AddRange(new[] { "g1", "g2" });
			store.SaveRosterItem(jid, r);
			store.SaveRosterItem(jid, r);

			roster = store.GetRosterItems(jid);
			Assert.AreEqual(1, roster.Count);
			Assert.AreEqual("x", roster[0].Jid.ToString());
			Assert.AreEqual("y", roster[0].Name);
			Assert.AreEqual(SubscriptionType.from, roster[0].Subscribtion);
			Assert.AreEqual(AskType.subscribe, roster[0].Ask);
			CollectionAssert.AreEqual(new[] { "g1", "g2" }, roster[0].Groups);

			r = store.GetRosterItem(jid, new Jid("x"));
			Assert.IsNotNull(r);

			roster = store.GetRosterItems(jid, SubscriptionType.from);
			Assert.AreEqual(1, roster.Count);
			roster = store.GetRosterItems(jid, SubscriptionType.both);
			Assert.AreEqual(0, roster.Count);

			foreach (var i in store.GetRosterItems(jid)) store.RemoveRosterItem(jid, i.Jid);

			store.SaveRosterItem(jid, new UserRosterItem(new Jid("x")));
			roster = store.GetRosterItems(jid);
			Assert.AreEqual(1, roster.Count);
			Assert.AreEqual("x", roster[0].Jid.ToString());
			Assert.AreEqual("x", roster[0].Name);
			Assert.AreEqual(default(SubscriptionType), roster[0].Subscribtion);
			Assert.AreEqual(default(AskType), roster[0].Ask);
			CollectionAssert.IsEmpty(roster[0].Groups);
		}
	}
}
