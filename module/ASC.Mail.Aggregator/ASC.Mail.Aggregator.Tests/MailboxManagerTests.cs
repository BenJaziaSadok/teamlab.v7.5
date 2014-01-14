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
using System.Net.Mail;
using ActiveUp.Net.Mail;
using NUnit.Framework;

namespace ASC.Mail.Aggregator.Tests
{
    [TestFixture]
    class MailboxManagerTests
    {
// ReSharper disable InconsistentNaming
        private const int TIME_LIMIT = 45000;
// ReSharper restore InconsistentNaming
        private MailBoxManager mailBoxManager;
        [SetUp]
        public void Setup()
        {
            mailBoxManager = new MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 30);
        }

        [Test]
        public void PerformaceTestForSimpleAddingSettingsSearch()
        {
            const int repeat_times = 5;
            var summ = 0.0;
            for (var i = 0; i < repeat_times; ++i)
            {
                var start_time = DateTime.Now;
                try
                {
                    mailBoxManager.SearchMailboxSettings("eva.mendes.4test@mail.ru", "Isadmin123", "", 1);
                } catch{}
                var diff = (DateTime.Now - start_time).TotalMilliseconds;
                Console.WriteLine(diff);
                summ += diff;
            }
            double average_time = summ / repeat_times;
            Console.Write(average_time);
            Assert.LessOrEqual(average_time, TIME_LIMIT);
        }

        [Test]
        public void PerformaceTestForBadMailbox()
        {
            const int repeat_times = 10;
            var summ = 0.0;
            for (var i = 0; i < repeat_times; ++i)
            {
                var start_time = DateTime.Now;
                try
                {
                    mailBoxManager.SearchMailboxSettings("eva.mendes.4test@qqqqqmail.ru", "Isadmin123", "", 1);
                } catch{}
                var diff = (DateTime.Now - start_time).TotalMilliseconds;
                summ += diff;
            }
            var average_time = summ / repeat_times;
            Console.Write(average_time);
            Assert.LessOrEqual(average_time, TIME_LIMIT);
        }

        [Test]
        public void SuccessTest()
        {
            var start_time = DateTime.Now;
            var mbox = mailBoxManager.SearchMailboxSettings("eva.mendes.4test@mail.ru", "Isadmin123", "", 1);
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNotNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void WrongPasswordTest()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("eva.mendes.4test@mail.ru", "Isadmin1234", "", 1);
            }
            catch (Exception)
            {}
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void WrongAccountTest()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("eva.mendesasdasdasd@mail.ru", "Isadmin123", "", 1);
            }
            catch (Exception)
            { }
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void BadSiteTest()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("qqqqqq@dropbox.com", "test_pass", "", 1);
            }
            catch (Exception)
            { }
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void YandexTest()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("asc.test.mail@yandex.ru", "Isadmin123", "", 1);
            }
            catch (Exception)
            { }
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNotNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        [ExpectedException("ASC.Mail.Aggregator.SmtpConnectionException")]
        public void TestForExceptions()
        {
            mailBoxManager.SearchMailboxSettings("eva.mendes.4test@mail.ru", "Isadmin1234", "", 1);
        }

        [Test]
        public void HotmailTest()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("profi.troll@hotmail.com", "Isadmin123", "", 1);
            }
            catch (Exception)
            { }
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNotNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void OutlookTest()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("profi.troll@outlook.com", "Isadmin123", "", 1);
            }
            catch (Exception)
            { }
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNotNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void GmailTest()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("asc4test@gmail.com", "Isadmin123", "", 1);
            }
            catch (Exception)
            { }
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNotNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void GmailTest2()
        {
            var start_time = DateTime.Now;
            MailBox mbox = null;
            try
            {
                mbox = mailBoxManager.SearchMailboxSettings("profi.troll.4test@gmail.com", "Isadmin123", "", 1);
            }
            catch (Exception)
            { }
            var diff = (DateTime.Now - start_time).TotalMilliseconds;
            Console.WriteLine(diff);
            Assert.IsNotNull(mbox);
            Assert.LessOrEqual(diff, TIME_LIMIT);
        }

        [Test]
        public void AolStartTlsTest()
        {
            var profi_trol = new MailBox
                {
                    Name = "",
                    EMail = new MailAddress("profi.troll@aol.com"),
                    
                    Account = "profi.troll",
                    Password = "Isadmin123",
                    AuthenticationTypeIn = SaslMechanism.Login,
                    IncomingEncryptionType = EncryptionType.StartTLS,
                    Imap = true,
                    Port = 143,
                    Server = "imap.aol.com",

                    SmtpAccount = "profi.troll",
                    SmtpPassword = "Isadmin123",
                    AuthenticationTypeSmtp = SaslMechanism.Login,
                    OutcomingEncryptionType = EncryptionType.StartTLS,
                    SmtpPort = 587,
                    SmtpServer = "smtp.aol.com"
                };
            Assert.IsTrue(profi_trol.Test());
        }
    }
}
