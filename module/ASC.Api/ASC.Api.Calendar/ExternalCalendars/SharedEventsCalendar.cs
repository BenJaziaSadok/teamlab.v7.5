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
using System.Linq;
using ASC.Api.Calendar.BusinessObjects;
using ASC.Core;
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.ExternalCalendars
{
    public class SharedEventsCalendar : BaseCalendar
    {
        public static string CalendarId { get { return "shared_events"; } }
        public static EventAlertType AlertType { get { return EventAlertType.Hour; } }
        public static TimeZoneInfo CalendarTimeZone
        {
            get
            {
                var t = CoreContext.TenantManager.GetCurrentTenant(false);
                return t != null ? t.TimeZone : TimeZoneInfo.Utc;
            }
        }

        public SharedEventsCalendar()
        {
            this.Id = CalendarId;
            this.Context.HtmlBackgroundColor = "#0797ba";
            this.Context.HtmlTextColor = "#000000";
            this.Context.GetGroupMethod = delegate() { return Resources.CalendarApiResource.PersonalCalendarsGroup; };
            this.Context.CanChangeTimeZone = true;
            this.Context.CanChangeAlertType = true;
            this.EventAlertType = AlertType;
            this.TimeZone = SharedEventsCalendar.CalendarTimeZone;
            this.SharingOptions.SharedForAll = true;
        }

        public override string Description
        {
            get { return Resources.CalendarApiResource.SharedEventsCalendarDescription; }
        }

        public override List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            using (var dataProvider = new DataProvider())
            {
                var events = dataProvider.LoadSharedEvents(userId, CoreContext.TenantManager.GetCurrentTenant().TenantId, utcStartDate, utcEndDate);
                events.ForEach(e => e.CalendarId = this.Id);
                var ievents = new List<IEvent>(events.Select(e => (IEvent)e));
                return ievents;
            }
        }

        public override string Name
        {
            get { return Resources.CalendarApiResource.SharedEventsCalendarName; }
        }

        public override Guid OwnerId
        {
            get
            {
                return SecurityContext.CurrentAccount.ID;
            }
        }
    }
}
