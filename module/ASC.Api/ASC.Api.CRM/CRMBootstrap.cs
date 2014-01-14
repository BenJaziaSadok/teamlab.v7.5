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
using ASC.Api.Interfaces;
using ASC.Core;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Web.Core;
using ASC.Web.Core.Calendars;
using ASC.Web.Files.Api;

namespace ASC.Api.CRM
{
    public class CRMBootstrap : IApiBootstrapper
    {
        public void Configure()
        {
            if (!FilesIntegration.IsRegisteredFileSecurityProvider("crm", "crm_common"))
            {
                FilesIntegration.RegisterFileSecurityProvider("crm", "crm_common", new FileSecurityProvider());
            }
            
            //Register prodjects' calendar events
            CalendarManager.Instance.RegistryCalendarProvider(userid =>
            {
                if (WebItemSecurity.IsAvailableForUser(WebItemManager.CRMProductID.ToString(), userid))
                {
                    var factory = new DaoFactory(CoreContext.TenantManager.GetCurrentTenant().TenantId, CRMConstants.DatabaseId);
                    return new List<BaseCalendar> { new CRMCalendar(factory, userid) };
                }
                else
                {
                    return new List<BaseCalendar>();
                }
            });
        }
    }
}
