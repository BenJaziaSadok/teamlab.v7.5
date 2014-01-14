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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.Core.Common.Tests
{
    [TestClass]
    public class ClientPaymentManagerTest
    {
        private readonly ClientPaymentManager paymentManager = new ClientPaymentManager(null, null, null);


        [TestMethod]
        public void ActivateCuponTest()
        {
            CoreContext.TenantManager.SetCurrentTenant(0);
            paymentManager.ActivateKey("IAALKCPBRY9ZSDLJZ4E2");
        }

        [TestMethod]
        public void GetPartnerByCupon()
        {
            paymentManager.GetPartner("WRFWGF6H2S7LBVS7WB01");
        }

        [TestMethod]
        public void CreateButton()
        {
            //var url = paymentManager.GetButton(-48, "f4cd3678-4725-4826-ab50-a0704d3295c2");
        }
    }
}
#endif