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

using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using System;
using System.Collections.Generic;

namespace ASC.Core
{
    public interface IPaymentManagerClient
    {
        Tariff GetTariff(int tenantId);

        void SetTariff(int tenantId, Tariff tariff);

        IEnumerable<PaymentInfo> GetTariffPayments(int tenant);

        IEnumerable<PaymentInfo> GetTariffPayments(int tenantId, DateTime from, DateTime to);

        Invoice GetPaymentInvoice(string paymentId);

        Uri GetShoppingUri(int tenant, int quotaId);

        void SendTrialRequest(int tenant, UserInfo user);

        void ActivateKey(string key);

        void RequestClientPayment(string partnerId, int quotaId, bool requestKey);

        Partner GetPartner(string key);

        IEnumerable<TenantQuota> GetPartnerTariffs(string partnerId);

        string GetButton(string partnerId, int quotaId);

        void CreateClient(string partnerId, string email, string firstName, string lastName, string phone, string portal, string portalDomain);
    }
}
