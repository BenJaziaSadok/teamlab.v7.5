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
namespace ASC.Core.Common.Tests
{
    using System;
    using System.Linq;
    using ASC.Common.Security.Authorizing;
    using ASC.Core.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DbAzServiceTest : DbBaseTest<DbAzService>
    {
        [ClassInitialize]
        public void ClearData()
        {
            foreach (var ac in Service.GetAces(Tenant, default(DateTime)))
            {
                if (ac.Tenant == Tenant) Service.RemoveAce(Tenant, ac);
            }
        }

        [TestMethod]
        public void AceRecords()
        {
            var ar1 = new AzRecord(Guid.Empty, Guid.Empty, AceType.Allow);
            Service.SaveAce(Tenant, ar1);

            var ar2 = new AzRecord(new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002"), AceType.Allow);
            Service.SaveAce(Tenant, ar2);

            var list = Service.GetAces(Tenant, default(DateTime)).ToList();

            CompareAces(ar1, list[list.IndexOf(ar1)]);
            CompareAces(ar2, list[list.IndexOf(ar2)]);

            Service.RemoveAce(Tenant, ar1);
            Service.RemoveAce(Tenant, ar2);

            list = Service.GetAces(Tenant, new DateTime(1900, 1, 1)).ToList();
            CollectionAssert.DoesNotContain(list, ar1);
            CollectionAssert.DoesNotContain(list, ar2);
        }

        private void CompareAces(AzRecord ar1, AzRecord ar2)
        {
            Assert.AreEqual(ar1.ActionId, ar2.ActionId);
            Assert.AreEqual(ar1.ObjectId, ar2.ObjectId);
            Assert.AreEqual(ar1.Reaction, ar2.Reaction);
            Assert.AreEqual(ar1.SubjectId, ar2.SubjectId);
            Assert.AreEqual(ar1.Tenant, ar2.Tenant);
        }
    }
}
#endif
