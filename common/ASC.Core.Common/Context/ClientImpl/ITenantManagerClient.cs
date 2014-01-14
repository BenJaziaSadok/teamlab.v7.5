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
    public interface ITenantManagerClient
    {
        List<Tenant> GetTenants();

        Tenant GetTenant(int tenantId);

        Tenant GetTenant(string domain);

        Tenant SetTenantVersion(Tenant tenant, int version);

        Tenant SaveTenant(Tenant tenant);

        void RemoveTenant(int tenantId);

        void CheckTenantAddress(string address);

        IEnumerable<TenantVersion> GetTenantVersions();


        Tenant GetCurrentTenant();

        Tenant GetCurrentTenant(bool throwOnError);

        void SetCurrentTenant(Tenant tenant);

        void SetCurrentTenant(int tenantId);

        void SetCurrentTenant(string domain);


        IEnumerable<TenantQuota> GetTenantQuotas();

        IEnumerable<TenantQuota> GetTenantQuotas(bool all);

        TenantQuota GetTenantQuota(int tenant);

        void SetTenantQuotaRow(TenantQuotaRow row, bool exchange);

        List<TenantQuotaRow> FindTenantQuotaRows(TenantQuotaRowQuery query);
    }
}
