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
    public class CalendarContext : ICloneable
    {
        public delegate string GetString();
        public GetString GetGroupMethod { get; set; }
        public string Group { get { return GetGroupMethod != null ? GetGroupMethod() : ""; } }
        public string HtmlTextColor { get; set; }
        public string HtmlBackgroundColor { get; set; }
        public bool CanChangeTimeZone { get; set; }
        public bool CanChangeAlertType { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }

    public interface ICalendar: IiCalFormatView
    {
        string Id { get;}
        string Name { get; }
        string Description { get; }        
        Guid OwnerId { get; }
        EventAlertType EventAlertType { get; }
        List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate);
        SharingOptions SharingOptions { get; }
        TimeZoneInfo TimeZone { get; }

        CalendarContext Context {get;}
    }
}
