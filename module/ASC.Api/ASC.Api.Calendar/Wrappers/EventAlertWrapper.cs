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
using ASC.Web.Core.Calendars;

namespace ASC.Api.Calendar.Wrappers
{
   
    [DataContract(Name = "alert", Namespace = "")]
    public class EventAlertWrapper
    {
        [DataMember(Name = "type")]
        public int Type{ get; set; }

        public static EventAlertWrapper ConvertToTypeSurrogated(EventAlertType type)
        {
            return new EventAlertWrapper() { Type = (int)type };
        }

        public static object GetSample()
        {
            return new { type = -1 };
        }
    }
}
