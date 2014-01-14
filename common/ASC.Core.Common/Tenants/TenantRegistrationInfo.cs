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
using System.Globalization;

namespace ASC.Core.Tenants
{
    public class TenantRegistrationInfo
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public CultureInfo Culture { get; set; }

        public TimeZoneInfo TimeZoneInfo { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string MobilePhone { get; set; }

        public string Password { get; set; }

        public string HostedRegion { get; set; }

        public string PartnerId { get; set; }


        public TenantRegistrationInfo()
        {
            Culture = CultureInfo.CurrentCulture;
            TimeZoneInfo = TimeZoneInfo.Local;
        }
    }
}
