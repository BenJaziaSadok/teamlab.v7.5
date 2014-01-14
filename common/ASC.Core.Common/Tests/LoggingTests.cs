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

#if DEBUG
using System;
using System.Runtime.Serialization;
using ASC.Core.Common.Logging;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.Core.Common.Tests
{
    [DataContract]
    class JsonTest
    {
        [DataMember]
        public string p1 { get; set; }

        [DataMember]
        public string p2 { get; set; }
    }

    [TestClass]
    public class LoggingTests
    {
        public LoggingTests()
        {
            XmlConfigurator.Configure();
        }


        [TestMethod]
        public void WriteTextTest()
        {
            AdminLog.PostAction("test {0} {1:Json}!", 4, Tuple.Create(5, "dddd"));

            CoreContext.TenantManager.SetCurrentTenant(0);
            SecurityContext.AuthenticateMe("maxim.kosov@avsmedia.net", "111111");
            AdminLog.PostAction("test2!");
        }

        [TestMethod]
        public void FormatTest()
        {
            var s = string.Format(new AdminLogFormatter(), "{0} {1:Json}", 3, new JsonTest { p1 = "abc", p2 = "8" });
            Assert.AreEqual("3 {\"p1\":\"abc\",\"p2\":\"8\"}", s);
        }
    }
}
#endif