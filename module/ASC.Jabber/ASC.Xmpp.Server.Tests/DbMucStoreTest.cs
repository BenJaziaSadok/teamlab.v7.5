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

using System.Collections.Generic;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.component;
using ASC.Xmpp.Server.Services.Muc2.Room.Member;
using ASC.Xmpp.Server.Services.Muc2.Room.Settings;
using ASC.Xmpp.Server.Storage;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
    [TestFixture]
    public class DbMucStoreTest : DbBaseTest
    {
        private DbMucStore store;

        public DbMucStoreTest()
        {
            store = new DbMucStore();
            store.Configure(GetConfiguration());
        }



        [Test]
        public void MucMessagesTest()
        {
            var room1 = new Jid("a.conf@s");
            var room2 = new Jid("b.conf@s");

            var m1 = new Message(new Jid("to1"), new Jid("from2"), "1");
            var m2 = new Message(new Jid("to2"), new Jid("from2"), "2");
            store.AddMucMessages(room1, new[] { m1, m2 });

            var mess = store.GetMucMessages(room2, 0);
            Assert.AreEqual(0, mess.Count);

            mess = store.GetMucMessages(room1, 0);
            Assert.AreEqual(2, mess.Count);
            Assert.AreEqual("1", mess[0].Body);
            Assert.AreEqual("2", mess[1].Body);

            mess = store.GetMucMessages(room1, 1);
            Assert.AreEqual(1, mess.Count);
            Assert.AreEqual("2", mess[0].Body);

            store.RemoveMucMessages(room1);
            mess = store.GetMucMessages(room1, 1);
            Assert.AreEqual(0, mess.Count);
        }

        [Test]
        public void MucRoomSettingsTest()
        {
            var room1 = new Jid("a.conf@s/R");
            store.RemoveMuc(room1);
            var s = store.GetMucRoomSettings(room1);
            Assert.IsNull(s);

            s = new MucRoomSettings();
            store.SetMucRoomSettings(room1, s);

            s = store.GetMucRoomSettings(room1);
            Assert.IsNotNull(s);

            s.Members = new List<MucRoomMemberInfo>();
            var m1 = new MucRoomMemberInfo("1:1:1");
            var m2 = new MucRoomMemberInfo("2:1:1");
            s.Members.Add(m1);
            s.Members.Add(m2);
            store.SetMucRoomSettings(room1, s);

            s = store.GetMucRoomSettings(room1);
            Assert.AreEqual(2, s.Members.Count);

            store.RemoveMuc(room1);
            s = store.GetMucRoomSettings(room1);
            Assert.IsNull(s);
        }
    }
}
