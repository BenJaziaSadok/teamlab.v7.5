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
    using System.Configuration;
    using ASC.Core.Billing;
    using ASC.Core.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TariffServiceTest
    {
        private readonly ITariffService tariffService;


        public TariffServiceTest()
        {
            var cs = ConfigurationManager.ConnectionStrings["core"];
            tariffService = new TariffService(cs, new DbQuotaService(cs), new DbTenantService(cs));
        }


        [TestMethod]
        public void TestShoppingUriBatch()
        {
            var bc = new BillingClient(true);
            var result = bc.GetPaymentUrls("0", new[] { "12", "13", "14", "0", "-2" });
        }

        [TestMethod]
        public void TestPaymentInfo()
        {
            var payments = tariffService.GetPayments(918, DateTime.MinValue, DateTime.MaxValue);
        }

        [TestMethod]
        public void TestTariff()
        {
            var tariff = tariffService.GetTariff(918);
        }

        [TestMethod]
        public void TestSetTariff()
        {
            var duedate = DateTime.UtcNow.AddMonths(1);
            tariffService.SetTariff(0, new Tariff { QuotaId = -1, DueDate = DateTime.MaxValue });
            tariffService.SetTariff(0, new Tariff { QuotaId = -21, DueDate = duedate });
            tariffService.SetTariff(0, new Tariff { QuotaId = -21, DueDate = duedate });
            tariffService.SetTariff(0, new Tariff { QuotaId = -1, DueDate = DateTime.MaxValue });
        }

        [TestMethod]
        public void TestInvoice()
        {
            var payments = tariffService.GetPayments(918, DateTime.MinValue, DateTime.MaxValue);
            foreach (var p in payments)
            {
                var invoice = tariffService.GetInvoice(p.CartId);
            }
        }
    }
}
#endif
