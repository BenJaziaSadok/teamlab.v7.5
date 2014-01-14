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
using System.Text;

namespace ASC.Web.Core.Calendars
{
    public delegate List<BaseCalendar> GetCalendarForUser(Guid userId);

    public class CalendarManager
    {   
        public static CalendarManager Instance
        {
            get;
            private set;
        }

        private List<GetCalendarForUser> _calendarProviders;
        private List<BaseCalendar> _calendars;

        static CalendarManager()
        {
            Instance = new CalendarManager();
        }

        private CalendarManager()
        {
            _calendars = new List<BaseCalendar>();
            _calendarProviders = new List<GetCalendarForUser>();
        }

        public void RegistryCalendar(BaseCalendar calendar)
        { 
            lock(this._calendars)
            {
                if (!this._calendars.Exists(c => String.Equals(c.Id, calendar.Id, StringComparison.InvariantCultureIgnoreCase)))
                    this._calendars.Add(calendar);
            }
        }

        public void UnRegistryCalendar(string calendarId)
        {
            lock (this._calendars)
            {
                this._calendars.RemoveAll(c => String.Equals(c.Id, calendarId, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public void RegistryCalendarProvider(GetCalendarForUser provider)
        {
            lock (this._calendarProviders)
            {
                if (!this._calendarProviders.Exists(p => p.Equals(provider)))
                    this._calendarProviders.Add(provider);
            }
        }

        public void UnRegistryCalendarProvider(GetCalendarForUser provider)
        {
            lock (this._calendarProviders)
            {
                this._calendarProviders.RemoveAll(p => p.Equals(provider));
            }
        }

        public BaseCalendar GetCalendarForUser(Guid userId, string calendarId)
        {
            return GetCalendarsForUser(userId).Find(c=> String.Equals(c.Id, calendarId, StringComparison.InvariantCultureIgnoreCase));
        }

        public List<BaseCalendar> GetCalendarsForUser(Guid userId)
        {
            var cals = new List<BaseCalendar>();
            foreach (var h in _calendarProviders)
            {
                var list =  h(userId);
                if(list!=null)
                    cals.AddRange(list.FindAll(c => c.SharingOptions.PublicForItem(userId)));
            }

            cals.AddRange(_calendars.FindAll(c => c.SharingOptions.PublicForItem(userId)));
            return cals;
        }

    }
}
