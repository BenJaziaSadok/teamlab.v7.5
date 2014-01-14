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
using System.Web;
namespace ASC.Web.Core.Calendars
{
    public abstract class BaseEvent : IEvent, ICloneable
    {
        internal TimeZoneInfo TimeZone { get; set; }

        public BaseEvent()
        {
            this.Context = new EventContext();
            this.AlertType = EventAlertType.Never;
            this.SharingOptions = new SharingOptions();
            this.RecurrenceRule = new RecurrenceRule();
        }

        #region IEvent Members

        public SharingOptions SharingOptions { get; set; }

        public virtual EventAlertType AlertType { get; set; }

        public virtual bool AllDayLong { get; set; }

        public virtual string CalendarId { get; set; }

        public virtual string Description { get; set; }

        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual Guid OwnerId { get; set; }

        public virtual DateTime UtcEndDate { get; set; }

        public virtual DateTime UtcStartDate { get; set; }

        public virtual EventContext Context { get; set; }

        public virtual RecurrenceRule RecurrenceRule { get; set; }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            var e = (BaseEvent)this.MemberwiseClone();
            e.Context = (EventContext)this.Context.Clone();
            e.RecurrenceRule = (RecurrenceRule)this.RecurrenceRule.Clone();
            e.SharingOptions = (SharingOptions)this.SharingOptions.Clone();
            return e;
        }

        #endregion


        #region IiCalFormatView Members

        public virtual string ToiCalFormat()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine(String.Format("UID:{0}", this.Id));
            sb.AppendLine(String.Format("SUMMARY:{0}", this.Name));

            if (!string.IsNullOrEmpty(this.Description))
                sb.AppendLine(String.Format("DESCRIPTION:{0}", this.Description.Replace("\n","\\n")));

            if (this.AllDayLong)
            {
                DateTime startDate = this.UtcStartDate, endDate = this.UtcEndDate;
                if (this.TimeZone != null)
                {
                    if (this.UtcStartDate != DateTime.MinValue)
                        startDate = startDate + TimeZone.BaseUtcOffset;

                    if(this.UtcEndDate != DateTime.MinValue)
                        endDate = endDate + TimeZone.BaseUtcOffset;
                }

                if (this.UtcStartDate != DateTime.MinValue)
                    sb.AppendLine(String.Format("DTSTART;VALUE=DATE:{0}", startDate.ToString("yyyyMMdd")));

                if (this.UtcEndDate != DateTime.MinValue)
                    sb.AppendLine(String.Format("DTEND;VALUE=DATE:{0}", endDate.AddDays(1).ToString("yyyyMMdd")));
            }
            else
            {
                if (this.UtcStartDate != DateTime.MinValue)
                    sb.AppendLine(String.Format("DTSTART:{0}", this.UtcStartDate.ToString("yyyyMMdd'T'HHmmss'Z'")));

                if (this.UtcEndDate != DateTime.MinValue)
                    sb.AppendLine(String.Format("DTEND:{0}", this.UtcEndDate.ToString("yyyyMMdd'T'HHmmss'Z'")));
            }
            

            if (this.RecurrenceRule != null)
                sb.AppendLine(this.RecurrenceRule.ToiCalFormat());

            sb.Append("END:VEVENT");
            return sb.ToString();
        }

        #endregion     
        
    }
}
