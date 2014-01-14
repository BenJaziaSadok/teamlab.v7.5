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

using System;
using System.Collections.Generic;

namespace ASC.Core.Billing
{
    public interface ITariffService
    {
        Tariff GetTariff(int tenantId, bool withRequestToPaymentSystem = true);

        void SetTariff(int tenantId, Tariff tariff);

        void ClearCache(int tenantId);

        IEnumerable<PaymentInfo> GetPayments(int tenantId, DateTime from, DateTime to);

        Uri GetShoppingUri(int tenant, int plan);

        Invoice GetInvoice(string paymentId);

        string GetButton(int tariffId, string partnerId);

        void SaveButton(int tariffId, string partnerId, string buttonUrl);
    }
}
