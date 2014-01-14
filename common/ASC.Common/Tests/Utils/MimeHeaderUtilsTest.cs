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
using ASC.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.Common.Tests.Utils
{
    [TestClass]
    public class MimeHeaderUtilsTest
    {
        [TestMethod]
        public void Encode()
        {
            Assert.AreEqual("=?utf-8?B?0YrRitGK?=", MimeHeaderUtils.EncodeMime("ъъъ"));
            Assert.AreEqual("ddd", MimeHeaderUtils.EncodeMime("ddd"));
        }
    }
}
#endif