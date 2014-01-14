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

#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASC.Web.Core.Calendars;
using ASC.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core;
using ASC.Core.Tenants;
using ASC.Web.Core;

#endregion

namespace ASC.Api.CRM
{

    public class CRMCalendar : BaseCalendar
    {
        private class Event : BaseEvent { }

        private Guid _userId;
        private DaoFactory _daoFactory;

        public CRMCalendar(DaoFactory daoFactory, Guid userId)
        {
            _userId = userId;
            _daoFactory = daoFactory;

            this.Context.HtmlBackgroundColor = "";
            this.Context.HtmlTextColor = "";
            this.Context.CanChangeAlertType = false;
            this.Context.CanChangeTimeZone = false;
            this.Context.GetGroupMethod = delegate() { return ASC.Web.CRM.Resources.CRMCommonResource.ProductName; };
            this.Id = "crm_calendar";
            this.EventAlertType = EventAlertType.Never;
            this.Name = ASC.Web.CRM.Resources.CRMCommonResource.ProductName;
            this.Description = "";
            this.SharingOptions = new SharingOptions();
            this.SharingOptions.PublicItems.Add(new SharingOptions.PublicItem() { Id = userId, IsGroup = false });
        }

        public override List<IEvent> LoadEvents(Guid userId, DateTime startDate, DateTime endDate)
        {
            var events = new List<IEvent>();
            
            if (!WebItemSecurity.IsAvailableForUser(WebItemManager.CRMProductID.ToString(), SecurityContext.CurrentAccount.ID))
            {
                return events;
            }

            var tasks = _daoFactory.GetTaskDao().GetTasks(String.Empty, userId, 0, false, DateTime.MinValue, DateTime.MinValue, EntityType.Any, 0, 0, 0, null);

            foreach (var t in tasks)
            {
                if (t.DeadLine != DateTime.MinValue)
                {
                    var e = new Event()
                    {
                        AlertType = EventAlertType.Never,
                        CalendarId = this.Id,
                        UtcStartDate = t.DeadLine,
                        UtcEndDate = t.DeadLine,
                        Id = t.ID.ToString(),
                        Name = ASC.Web.CRM.Resources.CRMCommonResource.ProductName + ": " + t.Title,
                        Description = t.Description
                    };

                    if (t.DeadLine.Hour == 0 && t.DeadLine.Minute == 0)
                        e.AllDayLong = true;

                    events.Add(e);
                }
            }

            return events;
        }

        public override TimeZoneInfo TimeZone
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TimeZone; }
        }
    }
}
