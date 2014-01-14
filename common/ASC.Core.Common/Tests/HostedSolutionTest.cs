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
    using System.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HostedSolutionTest
    {
        [TestMethod]
        public void FindTenants()
        {
            var h = new HostedSolution(ConfigurationManager.ConnectionStrings["core"]);
            var tenants = h.FindTenants("76ff727b-f987-4871-9834-e63d4420d6e9");
            Assert.AreNotEqual(0, tenants.Count);
        }

        [TestMethod]
        public void RegionsTest()
        {
            var regionSerice = new MultiRegionHostedSolution("site");

            var t1 = regionSerice.GetTenant("teamlab.com", 50001);
            Assert.AreEqual("alias_test2.teamlab.com", t1.TenantDomain);

            var t2 = regionSerice.GetTenant("teamlab.eu.com", 50001);
            Assert.AreEqual("tscherb.teamlab.eu.com", t2.TenantDomain);
        }
    }
}
#endif
