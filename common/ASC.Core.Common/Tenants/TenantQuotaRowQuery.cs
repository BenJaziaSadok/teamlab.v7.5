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

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantQuotaRowQuery
    {
        public int Tenant
        {
            get;
            private set;
        }

        public string Path
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }


        public TenantQuotaRowQuery(int tenant)
        {
            Tenant = tenant;
        }


        public TenantQuotaRowQuery WithPath(string path)
        {
            Path = path;
            return this;
        }

        public TenantQuotaRowQuery WithLastModified(DateTime lastModified)
        {
            LastModified = lastModified;
            return this;
        }
    }
}