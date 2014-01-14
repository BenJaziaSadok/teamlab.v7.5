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
using System.Runtime.Serialization;

namespace ASC.Api.Calendar.Wrappers
{
    [DataContract(Name = "timeZone", Namespace = "")]
    public class TimeZoneWrapper
    {
        private TimeZoneInfo _timeZone;
        public TimeZoneWrapper(TimeZoneInfo timeZone)
        {
            _timeZone = timeZone;
        }

        [DataMember(Name = "name", Order = 0)]
        public string Name
        {
            get
            {
                return _timeZone.DisplayName;
            }
            set { }
        }

        [DataMember(Name = "id", Order = 0)]
        public string Id
        {
            get
            {
                return _timeZone.Id;
            }
            set { }
        }

        [DataMember(Name = "offset", Order = 0)]
        public int Offset
        {
            get
            {
                return (int)_timeZone.BaseUtcOffset.TotalMinutes;
            }
            set { }
        }

        public static object GetSample()
        {
            return new { offset = 0, id = "UTC", name = "UTC" };
        }


    }

}
