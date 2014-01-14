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
    class BadEncodingTests : MessageParserTestsBase
    {
        //
        //This test creates right parsing xml. Use it for new test adding.
        //

        [Test]
        [Ignore("This text need for right answers generation")]
        public void RecerateRight_ParsingResults()
        {
            var eml_files = Directory.GetFiles(test_folder_path, "*.eml")
                                     .Select(path => Path.GetFileName(path));

            foreach (var eml_file in eml_files)
            {
                var eml_message = Parser.ParseMessageFromFile(test_folder_path + eml_file);
                CreateRightResult(eml_message, right_parser_results_path + eml_file.Replace(".eml", ".xml"));
            }
        }

        [Test]
        [Ignore("This text need for right answers generation")]
        public void RecerateRight_ParsingResult()
        {
            var eml_file = "bad_content_disposition_in_attaches.eml";
            var eml_message = Parser.ParseMessageFromFile(test_folder_path + eml_file);
            CreateRightResult(eml_message, right_parser_results_path + eml_file.Replace(".eml", ".xml"));
        }


        [Test]
        public void BadEncodingTest()
        {
            Test("bad_encoding.eml");
        }


        [Test]
        public void BadEncoding2Test()
        {
            Test("bad_encoding_2.eml");
        }


        [Test]
        public void BadAddressNameEncodingTest()
        {
            Test("bad_address_name_encoding.eml");
        }


        [Test]
        public void BadCharsInSubject()
        {
            Test("bad_chars_in_subject.eml");
        }


        [Test]
        public void BadSubjectUtf8()
        {
            Test("bad_subject_utf8.eml");
        }


        [Test]
        public void EncriptedUtf8()
        {
            Test("utf8_encripted_teamlab.eml");
        }


        [Test]
        public void BadDecodedBody()
        {
            Test("kuponika_ru_bad_decoding_body.eml");
        }

        [Test]
        public void BadEncodedMailHundle()
        {
            Test("hundle_bad_encoding_mail.eml");
        }

        [Test]
        public void BadFromInQuotedPrintable()
        {
            Test("bad_quoted_printable_from_teamlab.eml");
        }
    }
}
