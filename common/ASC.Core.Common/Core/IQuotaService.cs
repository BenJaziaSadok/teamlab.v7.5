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

using System.Collections.Generic;
using ASC.Core.Tenants;

namespace ASC.Core
{
    public interface IQuotaService
    {
        IEnumerable<TenantQuota> GetTenantQuotas();

        TenantQuota GetTenantQuota(int id);

        TenantQuota SaveTenantQuota(TenantQuota quota);

        void RemoveTenantQuota(int id);

        
        IEnumerable<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query);

        void SetTenantQuotaRow(TenantQuotaRow row, bool exchange);
    }
}
