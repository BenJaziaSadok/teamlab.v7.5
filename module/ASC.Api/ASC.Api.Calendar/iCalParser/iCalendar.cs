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
using ASC.Api.Calendar.Resources;
using ASC.Web.Core.Calendars;
using System.IO;
using System.Net;
using ASC.Core;

namespace ASC.Api.Calendar.iCalParser
{
    public class iCalendar : BaseCalendar
    {
        public static iCalendar GetFromStream(TextReader reader)
        {
            var emitter = new iCalendarEmitter();
            var parser = new Parser(reader, emitter);
            parser.Parse();
            return emitter.GetCalendar();
        }

        public static iCalendar GetFromUrl(string url)
        {
            return GetFromUrl(url, null);
        }

        public static iCalendar GetFromUrl(string url, string calendarId)
        {
            var cache = new iCalendarCache();
            iCalendar calendar = null;
            if (calendarId != null)
                calendar = cache.GetCalendarFromCache(calendarId);

            if (calendar == null)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                WebResponse resp = req.GetResponse();

                var ms = new MemoryStream();
                resp.GetResponseStream().StreamCopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);

                using (var tempReader = new StreamReader(ms))
                {
                    var reader = new StringReader(tempReader.ReadToEnd());
                    calendar = GetFromStream(reader);
                    
                    if (calendar != null && calendarId != null)
                    {
                        tempReader.BaseStream.Seek(0, SeekOrigin.Begin);
                        cache.UpdateCalendarCache(calendarId, tempReader);
                    }                    
                }
            }

            if (calendar == null)
                throw new Exception(CalendarApiResource.WrongiCalFeedLink);

            return calendar;
        }


        public List<iCalEvent> Events { get; set; }

        public iCalendar()
        {
            this.Context.CanChangeAlertType = false;
            this.Context.CanChangeTimeZone = false;
            this.Context.GetGroupMethod = delegate() { return Resources.CalendarApiResource.iCalCalendarsGroup; };

            this.EventAlertType = EventAlertType.Never;
            this.Events = new List<iCalEvent>();            
        }

        public bool isEmptyName
        {
            get { return String.IsNullOrEmpty(_name);}
        }

        private string _name;
        public override string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                    return Resources.CalendarApiResource.NoNameCalendar;

                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private TimeZoneInfo _timeZone;
        public override TimeZoneInfo TimeZone
        {
            get
            {
                if (_timeZone != null)
                    return _timeZone;

                if (!String.IsNullOrEmpty(xTimeZone))
                {
                    _timeZone = OlsenTimeZoneConverter.OlsonTZId2TimeZoneInfo(xTimeZone);
                    return _timeZone;
                }

                if (String.IsNullOrEmpty(TZID))
                {
                    _timeZone = CoreContext.TenantManager.GetCurrentTenant().TimeZone;
                    return _timeZone;
                }


                _timeZone = OlsenTimeZoneConverter.OlsonTZId2TimeZoneInfo(TZID);
                return _timeZone;
            }
            set
            {
                _timeZone = value;
            }
        }
        
        public string TZID { get; set; }

        public string xTimeZone { get; set; }
       
        public override List<IEvent> LoadEvents(Guid userId, DateTime utcStartDate, DateTime utcEndDate)
        {
            return Events.Cast<IEvent>().ToList();
        }       
    }
}
