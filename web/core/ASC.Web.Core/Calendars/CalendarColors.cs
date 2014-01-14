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
    public class CalendarColors
    {
        public string BackgroudColor { get; set; }
        public string TextColor { get; set; }
        public static List<CalendarColors> BaseColors
        {
            get {
                return new List<CalendarColors>(){
                    new CalendarColors(){ BackgroudColor = "#e34603", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#f88e14", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#ffb403", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#9fbb4c", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#288e31", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#4cbb78", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#0797ba", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#1d5f99", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#4c76bb", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#3552d2", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#473388", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#884cbb", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#cb59ba", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#ca3083", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#e24e78", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#bf0036", TextColor="#ffffff"}
                };
            }
        }

    }
}
