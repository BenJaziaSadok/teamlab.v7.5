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
using System.Runtime.Serialization;
using ASC.Core.Tenants;

namespace ASC.Api.Settings
{
    [DataContract(Name = "settings", Namespace = "")]
    public class SettingsWrapper
    {
        [DataMember]
        public string Timezone { get; set; }

        [DataMember]
        public List<string> TrustedDomains { get; set; }

        [DataMember]
        public TenantTrustedDomainsType TrustedDomainsType { get; set; }

        [DataMember]
        public string Culture { get; set; }

        [DataMember]
        public TimeSpan UtcOffset { get; set; }


        [DataMember]
        public double UtcHoursOffset { get; set; }

        public static SettingsWrapper GetSample()
        {
            return new SettingsWrapper()
                       {
                           Culture = "en-US",
                           Timezone = TimeZoneInfo.Utc.ToString(),
                           TrustedDomains = new List<string>() {"mydomain.com"},
                           UtcHoursOffset = -8.5,
                           UtcOffset = TimeSpan.FromHours(-8.5)
                       };
        }
    }
}