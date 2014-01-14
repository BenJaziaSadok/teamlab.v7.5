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

using NUnit.Framework;

namespace ASC.Mail.Aggregator.Tests.MessageParserTests
{
    [TestFixture]
    class BadContentTests : MessageParserTestsBase
    {
        [Test]
        public void BadContentTypeTest()
        {
            Test("dostavka_ru_bad_content_type.eml");
        }


        [Test]
        public void EmbedImageTest()
        {
            Test("embed_image.eml");
        }


        [Test]
        public void JavaScriptTest()
        {
            Test("exo__with_javascript.eml");
        }


        [Test]
        public void HtmlCharsIntroductionTest()
        {
            Test("html_chars_in_introduction.eml");
        }

        [Test]
        public void ManyAttachmentsTest()
        {
            Test("letter_with_58_attachments.eml");
        }


        [Test]
        public void BadSanitazeTest()
        {
            Test("Mail_ru_bad_sanitaze.eml");
        }


        [Test]
        public void StyleWithClassesTest()
        {
            Test("mail_ru_style_with_classes.eml");
        }

        [Test]
        public void NoImageAfterSanitizeTest()
        {
            Test("message-no-image-after-sanitize.eml");
        }


        [Test]
        public void MailRuMessageTest()
        {
            Test("message_mailru.eml");
        }

        [Test]
        public void UnsanitizedImage1Test()
        {
            Test("message_mailru_with_unsanitized_images.eml");
        }


        [Test]
        public void UnsanitizedImage2Test()
        {
            Test("message_mailru_with_unsanitized_images2.eml");
        }

        [Test]
        public void MessageWithRussianAttachmentTest()
        {
            Test("message_with_russian_attachment.eml");
        }


        [Test]
        public void MessageWithSubmessagesTest()
        {
            Test("message_with_submessages.eml");
        }

        [Test]
        public void MessageWithUnknownDispositionMimePartsTest()
        {
            Test("message_with_UnknownDispositionMimeParts.eml");
        }


        [Test]
        public void OnlyTextBodyTest()
        {
            Test("only_text_body.eml");
        }


        [Test]
        public void SlashInAddressNameTest()
        {
            Test("slash_in_address_name.eml");
        }

        [Test]
        public void SotmarketSubjectTest()
        {
            Test("sotmarket-subject-err.eml");
        }


        [Test]
        public void BaseTagTest()
        {
            Test("test_base_tag.eml");
        }

        [Test]
        public void SendPreparedTest()
        {
            Test("test_send_prepared.eml");
        }


        [Test]
        public void WithBaseTagTest()
        {
            Test("with_base_tag.eml");
        }


        [Test]
        public void SkypeScrollEmailTest()
        {
            Test("yandex_skype_scroll_email.eml");
        }


        [Test]
        public void UnlimitedScrollTest()
        {
            Test("printdirect_ru__unlimit_scroll.eml");
        }

        [Test]
        public void BadSubjectEncodingTest()
        {
            Test("kulichiki.eml");
        }

        [Test]
        public void BadAttachContentDisposition()
        {
            Test("bad_content_disposition_in_attaches.eml");
        }
    }
}
