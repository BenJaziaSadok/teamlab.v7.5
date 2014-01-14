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
using System.Configuration;
using ASC.Xmpp.Server.storage;
using ASC.Xmpp.Server.storage.Interface;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests {

	[TestFixture]
	public class DbOfflineMessageStoreTest {

		private IOfflineStore store;
		private string userName = "james.bond";

		[TestFixtureSetUp]
		public void SetUp() {
			store = new DbOfflineStore(ConfigurationManager.ConnectionStrings["UserStore"]);
		}

		[Test]
		public void OfflineMessagesTest() {
			store.RemoveAllOfflineMessages(userName);
			var messages = store.GetOfflineMessages(userName);
			Assert.AreEqual(0, messages.Count);

			var m = new OfflineMessage(){
				To = "x",
				From = "y",
				When = DateTime.Now,
				Body = "xy"
			};
			store.SaveOfflineMessage(userName, m);

			messages = store.GetOfflineMessages(userName);
			Assert.AreEqual(1, messages.Count);
			Assert.AreEqual("x", messages[0].To);
			Assert.AreEqual("y", messages[0].From);
			Assert.AreNotEqual(DateTime.MinValue, messages[0].When);
			Assert.AreEqual("xy", messages[0].Body);

			store.SaveOfflineMessage(userName, m);
			messages = store.GetOfflineMessages(userName);
			Assert.AreEqual(2, messages.Count);

			store.RemoveAllOfflineMessages(userName);

			store.SaveOfflineMessage(userName, new OfflineMessage());
			messages = store.GetOfflineMessages(userName);
			Assert.AreEqual(1, messages.Count);
			Assert.IsNull(messages[0].To);
			Assert.IsNull(messages[0].From);
			Assert.AreEqual(DateTime.MinValue, messages[0].When);
			Assert.IsNull(messages[0].Body);
		}
	}
}
