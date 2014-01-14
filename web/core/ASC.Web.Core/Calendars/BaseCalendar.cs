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
using ASC.Core;

namespace ASC.Web.Core.Calendars
{
    public abstract class BaseCalendar : ICalendar, ICloneable
    {
        public BaseCalendar()
        {
            this.Context = new CalendarContext();
            this.SharingOptions = new SharingOptions();
        }

        #region ICalendar Members

        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual Guid OwnerId { get; set; }

        public virtual EventAlertType EventAlertType { get; set; }

        public abstract List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate);

        public virtual SharingOptions SharingOptions { get; set; }

        public virtual TimeZoneInfo TimeZone { get; set; }

        public virtual CalendarContext Context { get; set; }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var cal = (BaseCalendar)this.MemberwiseClone();
            cal.Context = (CalendarContext)this.Context.Clone();
            cal.SharingOptions = (SharingOptions)this.SharingOptions.Clone();
            return cal;
        }

        #endregion

        #region IiCalFormatView Members

        public string ToiCalFormat()
        {
            var sb = new StringBuilder();

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:TeamLab Calendar");
            sb.AppendLine("VERSION:2.0");
            
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine(String.Format("X-WR-CALNAME:{0}", Name));
            sb.AppendLine(String.Format("X-WR-TIMEZONE:{0}", OlsenTimeZoneConverter.TimeZoneInfo2OlsonTZId(TimeZone)));
            //tz
            sb.AppendLine("BEGIN:VTIMEZONE");
            sb.AppendLine(String.Format("TZID:{0}", OlsenTimeZoneConverter.TimeZoneInfo2OlsonTZId(TimeZone)));
            sb.AppendLine("END:VTIMEZONE");

            //events
            foreach (var e in LoadEvents(SecurityContext.CurrentAccount.ID, DateTime.MinValue, DateTime.MaxValue))
            {
                if (e is BaseEvent && e.GetType().GetCustomAttributes(typeof(AllDayLongUTCAttribute),true).Length==0)
                    (e as BaseEvent).TimeZone = TimeZone;

                sb.AppendLine(e.ToiCalFormat());
            }

            sb.Append("END:VCALENDAR");

            return sb.ToString();
        }

        #endregion
    }
}
