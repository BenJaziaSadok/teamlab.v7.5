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
    using ASC.Core.Billing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    [TestClass]
    public class TariffSyncServiceTest
    {
        private readonly ITariffSyncService tariffSyncService;


        public TariffSyncServiceTest()
        {
            tariffSyncService = new TariffSyncService();
        }

        [TestMethod]
        public void GetTeriffsTest()
        {
            var tariff = tariffSyncService.GetTariffs(70, null).FirstOrDefault(t => t.Id == -38);
            tariff = tariffSyncService.GetTariffs(70, null).FirstOrDefault(t => t.Id == -38);
            Assert.AreEqual(1024 * 1024 * 1024, tariff.MaxFileSize);
            tariff = tariffSyncService.GetTariffs(74, null).FirstOrDefault(t => t.Id == -38);
            Assert.AreEqual(100 * 1024 * 1024, tariff.MaxFileSize);
        }

        [TestMethod]
        public void SyncTest()
        {
            using (var wcfClient = new TariffSyncClient())
            {
                var tariffs = wcfClient.GetTariffs(74, null);
                Assert.IsTrue(tariffs.Any());
            }
        }
    }
}
#endif
