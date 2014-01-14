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

using System.IO;
using System.Linq;
using NUnit.Framework;
using ActiveUp.Net.Mail;

namespace ASC.Mail.Aggregator.Tests.MessageParserTests
{
    [TestFixture]
    class MessageDeserializationTest : MessageParserTestsBase
    {
        const string TestFileName = "test_serialization.eml";
        const string TestFilePath = test_folder_path + TestFileName;


        [SetUp]
        public void PrepareMessageForSerialization()
        {
            var test_mail = new Message
            {
                From = { Email = "test.mail@qip.ru", Name = Codec.RFC2047Encode("test") },
                To = { new Address { Email = "test.mail@qip.ru", Name = Codec.RFC2047Encode("name1") },
                       new Address { Email = "test.mail@gmail.com", Name = Codec.RFC2047Encode("name2") } },
                Subject = Codec.RFC2047Encode("test"),
                BodyText = { Charset = utf8_charset,
                            ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable,
                            Text = "test" },
                BodyHtml = { Charset = utf8_charset,
                             ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable,
                             Text = "<a href='www.teamlab.com'>test</a>" }
            };

            test_mail.StoreToFile(TestFilePath);
        }


        [TearDown]
        public void CleanUpSerializedFile()
        {
            File.Delete(TestFilePath);
        }


        //This test fails if data directory invalid or corrupted
        [Test]
        public void TestDirectoryPath()
        {
            using (new FileStream(TestFilePath, FileMode.Open)) { }
        }


        [Test]
        public void DeserializationOfSerializedMessageTest()
        {
            var test_mail = Parser.ParseMessageFromFile(TestFilePath);

            Assert.AreEqual("test.mail@qip.ru", test_mail.From.Email);
            Assert.AreEqual("test", test_mail.From.Name);
            Assert.IsTrue(test_mail.To.Count(mail => (mail.Email == "test.mail@qip.ru") && (mail.Name == "name1")) != 0);
            Assert.IsTrue(test_mail.To.Count(mail => (mail.Email == "test.mail@gmail.com") && (mail.Name == "name2")) != 0);

            Assert.AreEqual("test", test_mail.Subject);
            Assert.AreEqual(0, test_mail.Attachments.Count);

            Assert.AreEqual(utf8_charset, test_mail.BodyText.Charset);
            Assert.AreEqual("test\r\n", test_mail.BodyText.Text);
            Assert.AreEqual("test\r\n", test_mail.BodyText.TextStripped);
            Assert.AreEqual(BodyFormat.Text, test_mail.BodyText.Format);
            Assert.AreEqual(ContentTransferEncoding.QuotedPrintable, test_mail.BodyText.ContentTransferEncoding);

            Assert.AreEqual(utf8_charset, test_mail.BodyHtml.Charset);
            Assert.AreEqual("<a href='www.teamlab.com'>test</a>\r\n", test_mail.BodyHtml.Text);
            Assert.AreEqual("test\r\n", test_mail.BodyHtml.TextStripped);
            Assert.AreEqual(BodyFormat.Html, test_mail.BodyHtml.Format);
            Assert.AreEqual(ContentTransferEncoding.QuotedPrintable, test_mail.BodyHtml.ContentTransferEncoding);
        }
    }
}
