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

#region Usings

using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain.Reports;

#endregion

namespace ASC.Projects.Core.Domain
{
    public class TaskFilter : ReportFilter, ICloneable
    {
        public int? Milestone { get; set; }

        public bool Follow { get; set; }

        public string SortBy { get; set; }

        public bool SortOrder { get; set; }

        public string SearchText { get; set; }

        public long Offset { get; set; }

        public long Max { get; set; }

        public int LastId { get; set; }

        public bool MyProjects { get; set; }

        public bool MyMilestones { get; set; }

        public Guid? ParticipantId { get; set; }

        public TaskFilter()
        {
            Max = 150001;
        }

        public Dictionary<string, Dictionary<string, bool>> SortColumns
        {
            get
            {
                return new Dictionary<string, Dictionary<string, bool>>
                           {
                               {
                                   "Task", new Dictionary<string, bool>{{"deadline", true}, {"priority", false}, {"create_on", false}, {"start_date", false}, {"title", true}}
                                   },
                               {
                                   "Milestone", new Dictionary<string, bool> {{"deadline", true}, {"create_on", false}, {"title", true}}
                                   },
                               {
                                   "Project", new Dictionary<string, bool> {{"create_on", false}, {"title", true}}
                                   },
                               {
                                   "Message", new Dictionary<string, bool> {{"comments", false}, {"create_on", false}, {"title", true}}
                                   },
                               {
                                   "TimeSpend", new Dictionary<string, bool> {{"date", false}, {"hours", false}, {"note", true}}
                                   }
                           };
            }
        }

        public string ToXml()
        {
            return ReportFilterSerializer.ToXml(this);
        }

        public static TaskFilter FromXml(string xml)
        {
            return ReportFilterSerializer.FromXml(xml);
        }

        public string ToUri()
        {
            return ReportFilterSerializer.ToUri(this);
        }

        public static TaskFilter FromUri(string uri)
        {
            return ReportFilterSerializer.FromUri(uri);
        }

        public static TaskFilter FromUri(Uri uri)
        {
            return FromUri(uri.Query);
        }

        public object Clone()
        {
            return FromXml(ToXml());
        }
    }
}
