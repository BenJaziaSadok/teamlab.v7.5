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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Mail.Aggregator.Tests
{
    [TestFixture]
    class HtmlSanitizerTests
    {
        [Test]
        public void TestForScriptTagRemove()
        {
            const string html = "<html><head></head><body>ter\"><script>alert(document.cookie);</script> <scr<script>ipt>alert();</scr<script>ipt></body></html>";
            var res = HtmlSanitizer.SanitizeHtmlForEditor(HtmlSanitizer.Sanitize(html, false));
            Assert.AreEqual("<div>ter\"> <></div>", res);
        }
    }
}