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

namespace ASC.Core.Billing
{
    class TariffSyncServiceSection : ConfigurationSection
    {
        [ConfigurationProperty("period", DefaultValue = "4:0:0")]
        public TimeSpan Period
        {
            get { return (TimeSpan)this["period"]; }
            set { this["period"] = value; }
        }

        [ConfigurationProperty("connectionStringName", DefaultValue = "core")]
        public string ConnectionStringName
        {
            get { return (string)this["connectionStringName"]; }
            set { this["connectionStringName"] = value; }
        }

        public static TariffSyncServiceSection GetSection()
        {
            return (TariffSyncServiceSection)ConfigurationManager.GetSection("tariffs") ?? new TariffSyncServiceSection();
        }
    }
}
