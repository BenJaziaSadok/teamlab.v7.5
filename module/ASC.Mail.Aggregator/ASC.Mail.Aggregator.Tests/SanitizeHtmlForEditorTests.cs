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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ASC.Mail.Aggregator.Tests
{
    [TestFixture]
    class SanitizeHtmlForEditorTests
    {
        [Test]
        public void TestForRemovingHtml()
        {
            var html = "<html></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual(String.Empty, res);
        }

        [Test]
        public void TestForRemovingHead1()
        {
            var html = "<html><head></head></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual(String.Empty, res);
        }

        [Test]
        public void TestForRemovingHead2()
        {
            var html = "<html><head><style>some content inside styles</style></head></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual(String.Empty, res);
        }
        [Test]
        public void TestForRemovingHead3()
        {
            var html = "<html>\r\n<head> \r\n<style> \r\n some \r\n content \r\n inside \r\n styles \r\n </style> \r\n </head> \r\n</html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual(String.Empty, res);
        }

        [Test]
        public void TestForSimpleBodyReplace()
        {
            var html = "<body></body>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual("<div></div>", res);
        }

        [Test]
        public void TestForComplexBodyReplace()
        {
            var html = "<html><head></head><body></body></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual("<div></div>", res);
        }

        [Test]
        public void TestForWrongBodyReplacements()
        {
            var html = "<html><head></head><body>I used tag <body></body> thats problem.</body></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual("<div>I used tag <body></body> thats problem.</div>", res);
        }

        [Test]
        public void TestForWrongHtmlReplacements()
        {
            var html = "<html><head></head><body>I used tag <html></html> thats problem.</body></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual("<div>I used tag <html></html> thats problem.</div>", res);
        }

        [Test]
        public void TestForWrongHeadReplacements()
        {
            var html = "<html><head></head><body>I used tag <head></head> thats problem.</body></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual("<div>I used tag <head></head> thats problem.</div>", res);
        }

        [Test]
        public void TestForAttributeInBodySaving()
        {
            var html = "<body class='test'></body>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(html);
            Assert.AreEqual("<div class='test'></div>", res);
        }
    }
}
