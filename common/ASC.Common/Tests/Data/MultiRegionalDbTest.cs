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
using ASC.Common.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.Common.Tests.Data
{
    [TestClass]
    public class MultiRegionalDbTest
    {
        [TestMethod]
        public void ExecuteListTest()
        {
            var db = new MultiRegionalDbManager("core");
            var r1 = db.ExecuteList("select 1");
            Assert.IsTrue(r1.Count > 1);
        }
    }
}
#endif