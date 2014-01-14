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
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
	public class DbVCardTest:DbBaseTest
	{
		private DbVCardStore store;

        public DbVCardTest()
		{
			store = new DbVCardStore();
            store.Configure(GetConfiguration());
        }

		[Test]
		public void VCardTest()
		{
			var jid = new Jid("jid1", "s", "R1");
            var vcard = new Vcard();
            vcard.JabberId = jid;

            store.SetVCard(jid, vcard);
            var v = store.GetVCard(jid);
            Assert.IsNotNull(v);
            Assert.AreEqual(vcard.JabberId, v.JabberId);

            v = store.GetVCard(new Jid("jid2", "s", "R1"));
            Assert.IsNull(v);
		}
	}
}
