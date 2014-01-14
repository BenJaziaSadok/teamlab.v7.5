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

#if (DEBUG)
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ASC.Common.Tests.Notify
{
    class TestPatternFormatter : IPatternFormatter
    {
        public string[] GetTags(IPattern pattern)
        {
            throw new NotImplementedException();
        }

        public void FormatMessage(ASC.Notify.Messages.INoticeMessage message, ITagValue[] tagsValues)
        {
            throw new NotImplementedException();
        }
    }

    class PatternResource
    {
        internal static string resource1
        {
            get { return "resource1"; }
        }

        internal static string resource2
        {
            get { return "resource2"; }
        }

        internal static string resource3
        {
            get { return "resource3"; }
        }
    }

    [TestClass]
    public class XmlPatternProvider2Test
    {
        private readonly string xml =
            @"<patterns>
                <formatter type='ASC.Common.Tests.Notify.TestPatternFormatter, ASC.Common'/>
  
                <pattern id='id1' sender='email.sender'>
                    <subject resource='|resource1|ASC.Common.Tests.Notify.PatternResource, ASC.Common' />
                    <body styler='ASC.Notify.Textile.TextileStyler,ASC.Notify.Textile' resource='|resource2|ASC.Common.Tests.Notify.PatternResource, ASC.Common' />
                </pattern>
                <pattern id='id1' sender='messanger.sender'>
                    <subject />
                    <body styler='ASC.Notify.Textile.JabberStyler,ASC.Notify.Textile' resource='|resource3|ASC.Common.Tests.Notify.PatternResource, ASC.Common' />
                </pattern>

                <pattern id='id2'>
                    <body styler='ASC.Notify.Textile.JabberStyler,ASC.Notify.Textile'>NVelocity Template</body>
                </pattern>

                <pattern id='id3' reference='id2'/>
            </patterns>";


        [TestMethod]
        public void XmlParseTest()
        {
            var xmlPatternProvider2 = new XmlPatternProvider2(xml);

            var pattern = xmlPatternProvider2.GetPattern(new NotifyAction("id1"), "email.sender");
            Assert.IsNotNull(pattern);
            Assert.AreEqual("id1", pattern.ID);
            Assert.AreEqual(PatternResource.resource1, pattern.Subject);
            Assert.AreEqual(PatternResource.resource2, pattern.Body);
            Assert.AreEqual("ASC.Notify.Textile.TextileStyler,ASC.Notify.Textile", pattern.Styler);
            Assert.AreEqual("html", pattern.ContentType);

            pattern = xmlPatternProvider2.GetPattern(new NotifyAction("id1"), "messanger.sender");
            Assert.IsNotNull(pattern);
            Assert.AreEqual("id1", pattern.ID);
            Assert.AreEqual(string.Empty, pattern.Subject);
            Assert.AreEqual(PatternResource.resource3, pattern.Body);
            Assert.AreEqual("ASC.Notify.Textile.JabberStyler,ASC.Notify.Textile", pattern.Styler);
            Assert.AreEqual("html", pattern.ContentType);

            pattern = xmlPatternProvider2.GetPattern(new NotifyAction("id1"), null);
            Assert.IsNull(pattern);

            pattern = xmlPatternProvider2.GetPattern(new NotifyAction("id2"), "email.sender");
            Assert.IsNotNull(pattern);
            Assert.AreEqual("id2", pattern.ID);
            Assert.AreEqual(string.Empty, pattern.Subject);
            Assert.AreEqual("NVelocity Template", pattern.Body);
            Assert.AreEqual("ASC.Notify.Textile.JabberStyler,ASC.Notify.Textile", pattern.Styler);
            Assert.AreEqual("html", pattern.ContentType);

            pattern = xmlPatternProvider2.GetPattern(new NotifyAction("id2"), null);
            Assert.IsNotNull(pattern);

            pattern = xmlPatternProvider2.GetPattern(new NotifyAction("id3"), null);
            Assert.IsNotNull(pattern);

            var formatter = xmlPatternProvider2.GetFormatter(pattern);
            Assert.AreEqual(typeof(TestPatternFormatter), formatter.GetType());
        }
    }
}
#endif