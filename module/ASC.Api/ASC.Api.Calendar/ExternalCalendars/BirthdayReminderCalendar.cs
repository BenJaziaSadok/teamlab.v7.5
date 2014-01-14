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
using System.Web;
using ASC.Web.Core.Calendars;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Api.Calendar.ExternalCalendars
{
    public class BirthdayReminderCalendar : BaseCalendar
    {
        public readonly static string CalendarId = "users_birthdays";

        public BirthdayReminderCalendar()
        {
            this.Id = CalendarId;
            this.Context.HtmlBackgroundColor = "#f08e1c";
            this.Context.HtmlTextColor = "#000000";
            this.Context.GetGroupMethod = delegate(){return Resources.CalendarApiResource.CommonCalendarsGroup;};
            this.Context.CanChangeTimeZone = false;
            this.EventAlertType = EventAlertType.Day;
            this.SharingOptions.SharedForAll = true;
        }

        private class BirthdayEvent : BaseEvent
        {           
            public BirthdayEvent(string id, string name, DateTime birthday)
            {
                this.Id = "bde_"+id;
                this.Name= name;                
                this.OwnerId = Guid.Empty;
                this.AlertType = EventAlertType.Day;
                this.AllDayLong = true;
                this.CalendarId = BirthdayReminderCalendar.CalendarId;
                this.UtcEndDate = birthday;
                this.UtcStartDate = birthday;
                this.RecurrenceRule.Freq = Frequency.Yearly;
            }
        }
       
        public override List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            var events = new List<IEvent>();
            var usrs = CoreContext.UserManager.GetUsers().Where(u => u.BirthDate.HasValue).ToList();
            foreach (var usr in usrs)
            {
                DateTime bd;

                if (DateTime.DaysInMonth(utcStartDate.Year, usr.BirthDate.Value.Month) >= usr.BirthDate.Value.Day)
                {
                    bd = new DateTime(utcStartDate.Year, usr.BirthDate.Value.Month, usr.BirthDate.Value.Day);

                    if (bd >= utcStartDate && bd <= utcEndDate)
                    {
                        events.Add(new BirthdayEvent(usr.ID.ToString(), usr.DisplayUserName(), usr.BirthDate.Value));
                        continue;
                    }
                }

                if (DateTime.DaysInMonth(utcEndDate.Year, usr.BirthDate.Value.Month) >= usr.BirthDate.Value.Day)
                {
                    bd = new DateTime(utcEndDate.Year, usr.BirthDate.Value.Month, usr.BirthDate.Value.Day);

                    if (bd >= utcStartDate && bd <= utcEndDate)
                        events.Add(new BirthdayEvent(usr.ID.ToString(), usr.DisplayUserName(), usr.BirthDate.Value));
                }
            }
            return events;
        }

        public override string Name
        {
            get { return Resources.CalendarApiResource.BirthdayCalendarName; }
        }

        public override string Description
        {
            get { return Resources.CalendarApiResource.BirthdayCalendarDescription; }
        }
        
        public override TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.Utc; }
        }
    }
}
