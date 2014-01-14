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
using ASC.Core.Tenants;

namespace ASC.Core
{
    public interface ITenantService
    {
        void ValidateDomain(string domain);

        IEnumerable<Tenant> GetTenants(DateTime from);

        IEnumerable<Tenant> GetTenants(string login, string passwordHash);

        Tenant GetTenant(int id);

        Tenant GetTenant(string domain);

        Tenant SaveTenant(Tenant tenant);

        void RemoveTenant(int id);

        byte[] GetTenantSettings(int tenant, string key);

        void SetTenantSettings(int tenant, string key, byte[] data);

        IEnumerable<TenantVersion> GetTenantVersions();
    }
}
