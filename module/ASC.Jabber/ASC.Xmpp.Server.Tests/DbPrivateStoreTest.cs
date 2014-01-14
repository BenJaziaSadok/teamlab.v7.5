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
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
    public class DbPrivateStoreTest : DbBaseTest
	{
		private DbPrivateStore store;

        public DbPrivateStoreTest()
		{
			store = new DbPrivateStore();
            store.Configure(GetConfiguration());
		}

		[Test]
		public void PrivateStoreTest()
		{
            var jid = new Jid("n", "s", "r");

			var el = new Vcard();
			el.Fullname = "x";
            store.SetPrivate(jid, el);

			var el2 = (Vcard)store.GetPrivate(jid, new Vcard());
			Assert.AreEqual(el.Fullname, el2.Fullname);

			var el3 = store.GetPrivate(new Jid("n2", "s", "r"), new Vcard());
			Assert.IsNull(el3);

            var el4 = store.GetPrivate(jid, new Roster());
            Assert.IsNull(el4);
        }
	}
}
