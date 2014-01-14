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
using ASC.Xmpp.Core.protocol.component;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
	public class DbMsgArchiveTest : DbBaseTest
	{
		private DbMessageArchive store;

        public DbMsgArchiveTest()
		{
			store = new DbMessageArchive();
            store.Configure(GetConfiguration());
		}

		[Test]
		public void MessageLoggingTest()
		{
			var from = new Jid("jid1", "s", "R1");
			var to = new Jid("jid2", "s", "R2");

			CheckGetLogging(from, to, true);

			store.SetMessageLogging(from, to, false);
			CheckGetLogging(from, to, false);

			store.SetMessageLogging(from, to, true);
			CheckGetLogging(from, to, true);

			store.SetMessageLogging(to, from, false);
			CheckGetLogging(from, to, false);

			store.SetMessageLogging(to, from, true);
			CheckGetLogging(from, to, true);
		}

		private void CheckGetLogging(Jid from, Jid to, bool expected)
		{
			var actual = store.GetMessageLogging(from, to);
			Assert.AreEqual(expected, actual);

			actual = store.GetMessageLogging(to, from);
			Assert.AreEqual(expected, actual);
		}


		[Test]
		public void MessagesTest()
		{
			var from = new Jid("jid1", "s", "R1");
			var to = new Jid("jid2", "s", "R2");
			var m1 = new Message(to, from, "to --> from");
			var m2 = new Message(from, to, "from --> to");

			store.SaveMessages(new[] { m1, m2 });

			var messages = store.GetMessages(from, to, DateTime.MinValue, DateTime.MaxValue, 2);
			Assert.AreEqual(messages.Length, 2);
			Assert.AreEqual(m1.From, messages[0].From);
			Assert.AreEqual(m2.From, messages[1].From);

			messages = store.GetMessages(to, from, DateTime.MinValue, DateTime.MaxValue, 2);
			Assert.AreEqual(messages.Length, 2);
			Assert.AreEqual(m1.From, messages[0].From);
			Assert.AreEqual(m2.From, messages[1].From);
		}
	}
}
