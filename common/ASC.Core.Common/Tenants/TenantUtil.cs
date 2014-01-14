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
using System.Configuration;

namespace ASC.Core.Tenants
{
    public class TenantUtil
    {
        public static String GetBaseDomain(String hostedRegion)
        {
            var baseHost = ConfigurationManager.AppSettings["core.base-domain"];

            if (String.IsNullOrEmpty(hostedRegion)) return baseHost;
            if (String.IsNullOrEmpty(baseHost)) return baseHost;
            if (baseHost.IndexOf('.') == -1) return baseHost;

            var subdomain = baseHost.Remove(baseHost.IndexOf('.'));
            return hostedRegion.StartsWith(subdomain + ".") ? hostedRegion : String.Join(".", new[] { subdomain, hostedRegion.TrimStart('.') });
        }

        public static DateTime DateTimeFromUtc(DateTime dbDateTime)
        {
            return DateTimeFromUtc(CoreContext.TenantManager.GetCurrentTenant(), dbDateTime);
        }

        public static DateTime DateTimeFromUtc(Tenant tenant, DateTime dbDateTime)
        {
            return DateTimeFromUtc(tenant.TimeZone, dbDateTime);
        }

        public static DateTime DateTimeFromUtc(TimeZoneInfo timeZone, DateTime dbDateTime)
        {
            if (dbDateTime.Kind == DateTimeKind.Local)
            {
                dbDateTime = DateTime.SpecifyKind(dbDateTime, DateTimeKind.Unspecified);
            }
            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTime(dbDateTime, TimeZoneInfo.Utc, timeZone), DateTimeKind.Local);
        }

        public static DateTime DateTimeToUtc(DateTime dbDateTime)
        {
            return DateTimeToUtc(CoreContext.TenantManager.GetCurrentTenant(), dbDateTime);
        }

        public static DateTime DateTimeToUtc(Tenant tenant, DateTime dbDateTime)
        {
            return DateTimeToUtc(tenant.TimeZone, dbDateTime);
        }

        public static DateTime DateTimeToUtc(TimeZoneInfo timeZone, DateTime dbDateTime)
        {
            if (dbDateTime.Kind == DateTimeKind.Utc)
            {
                return dbDateTime;
            }
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dbDateTime, DateTimeKind.Unspecified), timeZone);

        }

        public static DateTime DateTimeNow()
        {
            return DateTimeNow(CoreContext.TenantManager.GetCurrentTenant());
        }

        public static DateTime DateTimeNow(Tenant tenant)
        {
            return DateTimeNow(tenant.TimeZone);
        }

        public static DateTime DateTimeNow(TimeZoneInfo timeZone)
        {
            return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone), DateTimeKind.Local);
        }
    }
}