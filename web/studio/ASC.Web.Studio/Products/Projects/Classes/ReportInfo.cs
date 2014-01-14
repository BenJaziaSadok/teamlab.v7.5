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

namespace ASC.Web.Projects.Classes
{
    [Serializable]
    public class ReportInfo
    {
        public static ReportInfo Empty = new ReportInfo(string.Empty, string.Empty, new string[0]);

        public string Description { get; set; }

        public string Title { get; set; }

        public string[] Columns { get; set; }

        public ReportInfo(string desc, string title, string[] columns)
        {
            Description = desc;
            Title = title;
            Columns = columns ?? new string[0];
        }
    }
}